import { useEffect, useMemo, useState } from 'react';
import {
  ColumnDef,
  SortingState,
  flexRender,
  getCoreRowModel,
  useReactTable
} from '@tanstack/react-table';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { Link, useNavigate } from 'react-router-dom';
import { PlusCircle, Pencil, Trash2, ArrowUpDown } from 'lucide-react';
import {
  fetchPosts,
  deletePost
} from '../../features/posts/api';
import {
  Post,
  PostManagementListResponse,
  PostTableFilters
} from '../../features/posts/types';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '../../components/ui/dialog';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Separator } from '../../components/ui/separator';
import { Badge } from '../../components/ui/badge';
import { handleApiError, showApiResponseError } from '../../lib/api-error';
import { cn } from '../../lib/utils';

const fieldMap: Record<string, string> = {
  id: 'Id',
  title: 'Title',
  categoryName: 'Category.Name',
  isPublished: 'IsPublished'
};

export function PostsPage() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const [filters, setFilters] = useState<PostTableFilters>({
    pageIndex: 0,
    pageSize: 10
  });
  const [searchTerm, setSearchTerm] = useState('');
  const [sorting, setSorting] = useState<SortingState>([
    { id: 'id', desc: true }
  ]);
  const [postToDelete, setPostToDelete] = useState<Post | null>(null);

  const postsQuery = useQuery<PostManagementListResponse>({
    queryKey: [
      'posts',
      filters.pageIndex,
      filters.pageSize,
      filters.search ?? '',
      filters.sort?.field ?? '',
      filters.sort?.dir ?? ''
    ],
    queryFn: () => fetchPosts(filters),
    placeholderData: (previousData) => previousData
  });

  useEffect(() => {
    const timeout = window.setTimeout(() => {
      setFilters((prev) => ({ ...prev, search: searchTerm, pageIndex: 0 }));
    }, 400);

    return () => window.clearTimeout(timeout);
  }, [searchTerm]);

  useEffect(() => {
    setFilters((prev) => {
      const sortState = sorting[0];
      const nextSort = sortState
        ? {
            field: fieldMap[sortState.id] ?? sortState.id,
            dir: sortState.desc ? ('desc' as const) : ('asc' as const)
          }
        : undefined;

      if (
        (prev.sort?.field ?? '') === (nextSort?.field ?? '') &&
        (prev.sort?.dir ?? '') === (nextSort?.dir ?? '')
      ) {
        return prev;
      }

      return {
        ...prev,
        sort: nextSort,
        pageIndex: 0
      };
    });
  }, [sorting]);

  const columns = useMemo<ColumnDef<Post>[]>(
    () => [
      {
        accessorKey: 'id',
        header: 'ID',
        cell: ({ row }) => <span className="font-mono text-xs text-muted-foreground">#{row.original.id}</span>,
        enableSorting: true
      },
      {
        accessorKey: 'title',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Başlık
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => <span className="font-medium">{row.original.title}</span>,
        enableSorting: true
      },
      {
        accessorKey: 'categoryName',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Kategori
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => row.original.categoryName,
        enableSorting: true
      },
      {
        accessorKey: 'isPublished',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Durum
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => (
          <Badge variant={row.original.isPublished ? 'default' : 'secondary'}>
            {row.original.isPublished ? 'Yayında' : 'Taslak'}
          </Badge>
        ),
        enableSorting: true
      },
      {
        accessorKey: 'summary',
        header: 'Özet',
        cell: ({ row }) => {
          const summary = row.original.summary;
          return summary.length > 80 ? `${summary.slice(0, 80)}...` : summary;
        },
        enableSorting: false
      },
      {
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="icon"
              onClick={() => navigate(`/admin/posts/${row.original.id}/edit`)}
              aria-label="Düzenle"
            >
              <Pencil className="h-4 w-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setPostToDelete(row.original)}
              aria-label="Sil"
            >
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          </div>
        )
      }
    ],
    [navigate, setPostToDelete]
  );

  const table = useReactTable({
    data: postsQuery.data?.items ?? [],
    columns,
    state: {
      sorting
    },
    onSortingChange: setSorting,
    manualSorting: true,
    getCoreRowModel: getCoreRowModel()
  });

  const deleteMutation = useMutation({
    mutationFn: (postId: number) => deletePost(postId),
    onSuccess: (result) => {
      if (!result.success) {
        showApiResponseError(result, 'Gönderi silinemedi');
        return;
      }
      toast.success(result.message || 'Gönderi silindi');
      setPostToDelete(null);
      queryClient.invalidateQueries({ queryKey: ['posts'] });
      queryClient.invalidateQueries({ queryKey: ['posts', 'published'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard-statistics'] });
      queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
    },
    onError: (error) => handleApiError(error, 'Gönderi silinemedi')
  });

  const handlePageChange = (direction: 'prev' | 'next') => {
    setFilters((prev) => {
      const nextIndex = direction === 'prev' ? Math.max(prev.pageIndex - 1, 0) : prev.pageIndex + 1;
      return {
        ...prev,
        pageIndex: nextIndex
      };
    });
  };

  const handlePageSizeChange = (value: number) => {
    setFilters((prev) => ({
      ...prev,
      pageSize: value,
      pageIndex: 0
    }));
  };

  const totalPages = postsQuery.data?.pages ?? 0;
  const currentPage = postsQuery.data?.index ?? filters.pageIndex;
  const isFirstPage = currentPage <= 0;
  const isLastPage = totalPages > 0 ? currentPage >= totalPages - 1 : false;

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <CardTitle>Gönderi Yönetimi</CardTitle>
            <p className="text-sm text-muted-foreground">
              Gönderilerinizi oluşturun, düzenleyin ve yönetin. Arama, sıralama ve sayfalama özelliklerini kullanın.
            </p>
          </div>
          <Button className="gap-2" asChild>
            <Link to="/admin/posts/new">
              <PlusCircle className="h-4 w-4" /> Yeni Gönderi
            </Link>
          </Button>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
            <Input
              placeholder="Gönderi ara..."
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              className="md:w-72"
            />
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <span>Sayfa başına:</span>
              <select
                className="rounded-md border bg-background px-2 py-1"
                value={filters.pageSize}
                onChange={(event) => handlePageSizeChange(Number(event.target.value))}
              >
                {[5, 10, 20, 50].map((size) => (
                  <option key={size} value={size}>
                    {size}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <div className="overflow-hidden rounded-lg border bg-background">
            <Table>
              <TableHeader>
                {table.getHeaderGroups().map((headerGroup) => (
                  <TableRow key={headerGroup.id}>
                    {headerGroup.headers.map((header) => (
                      <TableHead key={header.id} className={cn(header.column.id === 'actions' && 'w-[120px]')}>
                        {header.isPlaceholder
                          ? null
                          : flexRender(header.column.columnDef.header, header.getContext())}
                      </TableHead>
                    ))}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {postsQuery.isLoading ? (
                  <TableRow>
                    <TableCell colSpan={columns.length} className="h-24 text-center">
                      Veriler yükleniyor...
                    </TableCell>
                  </TableRow>
                ) : table.getRowModel().rows.length ? (
                  table.getRowModel().rows.map((row) => (
                    <TableRow key={row.id}>
                      {row.getVisibleCells().map((cell) => (
                        <TableCell key={cell.id}>
                          {flexRender(cell.column.columnDef.cell, cell.getContext())}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={columns.length} className="h-24 text-center">
                      Gönderi bulunamadı.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>
          <div className="flex flex-col items-center justify-between gap-3 border-t pt-4 text-sm text-muted-foreground md:flex-row">
            <div>
              Toplam {postsQuery.data?.count ?? 0} kayıt - Sayfa {currentPage + 1} / {totalPages || 1}
            </div>
            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm" onClick={() => handlePageChange('prev')} disabled={isFirstPage}>
                Önceki
              </Button>
              <Button variant="outline" size="sm" onClick={() => handlePageChange('next')} disabled={isLastPage}>
                Sonraki
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      <Dialog open={!!postToDelete} onOpenChange={(open) => !open && setPostToDelete(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Gönderiyi Sil</DialogTitle>
            <DialogDescription>
              Bu gönderiyi silmek istediğinizden emin misiniz? İşlem geri alınamaz.
            </DialogDescription>
          </DialogHeader>
          <Separator />
          <p className="text-sm text-muted-foreground">
            Silinecek gönderi: <span className="font-medium">{postToDelete?.title}</span>
          </p>
          <DialogFooter className="gap-2">
            <Button type="button" variant="ghost" onClick={() => setPostToDelete(null)}>
              İptal
            </Button>
            <Button
              type="button"
              variant="destructive"
              disabled={deleteMutation.isPending}
              onClick={() => postToDelete && deleteMutation.mutate(postToDelete.id)}
            >
              {deleteMutation.isPending ? 'Siliniyor...' : 'Sil'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
