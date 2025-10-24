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
import { Link } from 'react-router-dom';
import { PlusCircle, Pencil, Trash2, ArrowUpDown } from 'lucide-react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import {
  fetchPosts,
  updatePost,
  deletePost
} from '../../features/posts/api';
import {
  Post,
  PostFormValues,
  PostManagementListResponse,
  PostTableFilters
} from '../../features/posts/types';
import { postSchema, PostFormSchema } from '../../features/posts/schema';
import { getAllCategories } from '../../features/categories/api';
import { Category } from '../../features/categories/types';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '../../components/ui/dialog';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Separator } from '../../components/ui/separator';
import { Badge } from '../../components/ui/badge';
import { cn } from '../../lib/utils';

const fieldMap: Record<string, string> = {
  id: 'Id',
  title: 'Title',
  categoryName: 'Category.Name',
  isPublished: 'IsPublished'
};

const textareaBaseClasses =
  'flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50';

export function PostsPage() {
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState<PostTableFilters>({
    pageIndex: 0,
    pageSize: 10
  });
  const [searchTerm, setSearchTerm] = useState('');
  const [sorting, setSorting] = useState<SortingState>([
    { id: 'title', desc: false }
  ]);
  const [editingPost, setEditingPost] = useState<Post | null>(null);
  const [postToDelete, setPostToDelete] = useState<Post | null>(null);

  const categoriesQuery = useQuery<Category[]>({
    queryKey: ['categories-options'],
    queryFn: getAllCategories
  });

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
              onClick={() => setEditingPost(row.original)}
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
    []
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

  const updateMutation = useMutation({
    mutationFn: (values: PostFormValues) =>
      editingPost ? updatePost(editingPost.id, values) : Promise.reject(),
    onSuccess: (result) => {
      if (!result.success) {
        toast.error(result.message || 'Gönderi güncellenemedi');
        return;
      }
      toast.success(result.message || 'Gönderi güncellendi');
      setEditingPost(null);
      queryClient.invalidateQueries({ queryKey: ['posts'] });
      queryClient.invalidateQueries({ queryKey: ['posts', 'published'] });
    },
    onError: () => toast.error('Gönderi güncellenirken bir hata oluştu')
  });

  const deleteMutation = useMutation({
    mutationFn: (postId: number) => deletePost(postId),
    onSuccess: (result) => {
      if (!result.success) {
        toast.error(result.message || 'Gönderi silinemedi');
        return;
      }
      toast.success(result.message || 'Gönderi silindi');
      setPostToDelete(null);
      queryClient.invalidateQueries({ queryKey: ['posts'] });
      queryClient.invalidateQueries({ queryKey: ['posts', 'published'] });
    },
    onError: () => toast.error('Gönderi silinirken bir hata oluştu')
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

  const formMethods = useForm<PostFormSchema>({
    resolver: zodResolver(postSchema),
    defaultValues: {
      title: '',
      summary: '',
      body: '',
      thumbnail: '',
      isPublished: false,
      categoryId: 0
    }
  });

  const defaultFormValues = useMemo(
    () => ({
      title: '',
      summary: '',
      body: '',
      thumbnail: '',
      isPublished: false,
      categoryId: categoriesQuery.data?.[0]?.id ?? 0
    }),
    [categoriesQuery.data]
  );

  useEffect(() => {
    if (editingPost) {
      formMethods.reset({
        title: editingPost.title,
        summary: editingPost.summary,
        body: editingPost.body,
        thumbnail: editingPost.thumbnail ?? '',
        isPublished: editingPost.isPublished,
        categoryId: editingPost.categoryId
      });
    }
  }, [editingPost, formMethods]);

  const onSubmit = formMethods.handleSubmit(async (values) => {
    if (editingPost) {
      await updateMutation.mutateAsync(values);
    }
    formMethods.reset(defaultFormValues);
  });

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

      <Dialog open={!!editingPost} onOpenChange={(open) => !open && setEditingPost(null)}>
        <DialogContent className="max-w-3xl">
          <DialogHeader>
            <DialogTitle>Gönderiyi Düzenle</DialogTitle>
            <DialogDescription>Seçili gönderinin detaylarını güncelleyin.</DialogDescription>
          </DialogHeader>
          <form id="edit-post-form" onSubmit={onSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="edit-post-title">Başlık</Label>
              <Input
                id="edit-post-title"
                placeholder="Gönderi başlığı"
                {...formMethods.register('title')}
              />
              {formMethods.formState.errors.title && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.title.message}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-post-summary">Özet</Label>
              <textarea
                id="edit-post-summary"
                placeholder="Gönderinin kısa özeti"
                className={cn(textareaBaseClasses, 'min-h-[100px]')}
                {...formMethods.register('summary')}
              />
              {formMethods.formState.errors.summary && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.summary.message}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-post-body">İçerik</Label>
              <textarea
                id="edit-post-body"
                placeholder="Gönderi içeriğini yazın"
                className={cn(textareaBaseClasses, 'min-h-[180px]')}
                {...formMethods.register('body')}
              />
              {formMethods.formState.errors.body && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.body.message}</p>
              )}
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="edit-post-category">Kategori</Label>
                <select
                  id="edit-post-category"
                  className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                  {...formMethods.register('categoryId', { valueAsNumber: true })}
                >
                  <option value={0} disabled>
                    Kategori seçin
                  </option>
                  {categoriesQuery.data?.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))}
                </select>
                {formMethods.formState.errors.categoryId && (
                  <p className="text-sm text-destructive">{formMethods.formState.errors.categoryId.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="edit-post-thumbnail">Küçük Görsel URL</Label>
                <Input
                  id="edit-post-thumbnail"
                  placeholder="https://..."
                  {...formMethods.register('thumbnail')}
                />
                {formMethods.formState.errors.thumbnail && (
                  <p className="text-sm text-destructive">{formMethods.formState.errors.thumbnail.message}</p>
                )}
              </div>
            </div>
            <div className="flex items-center gap-2">
              <input
                id="edit-post-isPublished"
                type="checkbox"
                className="h-4 w-4 rounded border border-input"
                {...formMethods.register('isPublished')}
              />
              <Label htmlFor="edit-post-isPublished" className="text-sm font-medium">
                Gönderiyi yayınla
              </Label>
            </div>
          </form>
          <DialogFooter className="gap-2">
            <Button type="button" variant="ghost" onClick={() => setEditingPost(null)}>
              İptal
            </Button>
            <Button type="submit" form="edit-post-form" disabled={updateMutation.isPending}>
              {updateMutation.isPending ? 'Güncelleniyor...' : 'Güncelle'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

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
