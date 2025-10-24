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
import { PlusCircle, Pencil, Trash2, ArrowUpDown } from 'lucide-react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { fetchCategories, createCategory, updateCategory, deleteCategory } from '../../features/categories/api';
import {
  Category,
  CategoryFormValues,
  CategoryListResponse,
  CategoryTableFilters
} from '../../features/categories/types';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '../../components/ui/dialog';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Separator } from '../../components/ui/separator';
import { cn } from '../../lib/utils';

const categorySchema = z.object({
  name: z.string().min(2, 'Kategori adı en az 2 karakter olmalıdır')
});

type CategoryFormSchema = z.infer<typeof categorySchema>;

const fieldMap: Record<string, string> = {
  id: 'Id',
  name: 'Name'
};

export function CategoriesPage() {
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState<CategoryTableFilters>({
    pageIndex: 0,
    pageSize: 10
  });
  const [searchTerm, setSearchTerm] = useState('');
  const [sorting, setSorting] = useState<SortingState>([
    { id: 'name', desc: false }
  ]);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [categoryToDelete, setCategoryToDelete] = useState<Category | null>(null);

  const categoriesQuery = useQuery<CategoryListResponse>({
    queryKey: [
      'categories',
      filters.pageIndex,
      filters.pageSize,
      filters.search ?? '',
      filters.sort?.field ?? '',
      filters.sort?.dir ?? ''
    ],
    queryFn: () => fetchCategories(filters),
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

  const columns = useMemo<ColumnDef<Category>[]>(
    () => [
      {
        accessorKey: 'id',
        header: 'ID',
        cell: ({ row }) => <span className="font-mono text-xs text-muted-foreground">#{row.original.id}</span>,
        enableSorting: true
      },
      {
        accessorKey: 'name',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Kategori Adı
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => <span className="font-medium">{row.original.name}</span>,
        enableSorting: true
      },
      {
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setEditingCategory(row.original)}
              aria-label="Düzenle"
            >
              <Pencil className="h-4 w-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setCategoryToDelete(row.original)}
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
    data: categoriesQuery.data?.items ?? [],
    columns,
    state: {
      sorting
    },
    onSortingChange: setSorting,
    manualSorting: true,
    getCoreRowModel: getCoreRowModel()
  });

  const createMutation = useMutation({
    mutationFn: createCategory,
    onSuccess: (result) => {
      if (!result.success) {
        toast.error(result.message || 'Kategori eklenemedi');
        return;
      }
      toast.success(result.message || 'Kategori eklendi');
      setIsCreateOpen(false);
      queryClient.invalidateQueries({ queryKey: ['categories'] });
    },
    onError: () => toast.error('Kategori eklenirken bir hata oluştu')
  });

  const updateMutation = useMutation({
    mutationFn: (values: CategoryFormValues) =>
      editingCategory ? updateCategory(editingCategory.id, values) : Promise.reject(),
    onSuccess: (result) => {
      if (!result.success) {
        toast.error(result.message || 'Kategori güncellenemedi');
        return;
      }
      toast.success(result.message || 'Kategori güncellendi');
      setEditingCategory(null);
      queryClient.invalidateQueries({ queryKey: ['categories'] });
    },
    onError: () => toast.error('Kategori güncellenirken bir hata oluştu')
  });

  const deleteMutation = useMutation({
    mutationFn: (categoryId: number) => deleteCategory(categoryId),
    onSuccess: (result) => {
      if (!result.success) {
        toast.error(result.message || 'Kategori silinemedi');
        return;
      }
      toast.success(result.message || 'Kategori silindi');
      setCategoryToDelete(null);
      queryClient.invalidateQueries({ queryKey: ['categories'] });
    },
    onError: () => toast.error('Kategori silinirken bir hata oluştu')
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

  const formMethods = useForm<CategoryFormSchema>({
    resolver: zodResolver(categorySchema),
    defaultValues: {
      name: ''
    }
  });

  useEffect(() => {
    if (editingCategory) {
      formMethods.reset({ name: editingCategory.name });
    } else {
      formMethods.reset({ name: '' });
    }
  }, [editingCategory, formMethods]);

  const onSubmit = formMethods.handleSubmit(async (values) => {
    if (editingCategory) {
      await updateMutation.mutateAsync(values);
    } else {
      await createMutation.mutateAsync(values);
    }
    formMethods.reset({ name: '' });
  });

  const totalPages = categoriesQuery.data?.pages ?? 0;
  const currentPage = categoriesQuery.data?.index ?? filters.pageIndex;
  const isFirstPage = currentPage <= 0;
  const isLastPage = totalPages > 0 ? currentPage >= totalPages - 1 : false;

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <CardTitle>Kategori Yönetimi</CardTitle>
            <p className="text-sm text-muted-foreground">
              Kategorilerinizi oluşturun, düzenleyin ve yönetin. Arama, sıralama ve sayfalama özelliklerini kullanın.
            </p>
          </div>
          <Dialog
            open={isCreateOpen}
            onOpenChange={(open) => {
              setIsCreateOpen(open);
              if (!open && !editingCategory) {
                formMethods.reset({ name: '' });
              }
            }}
          >
            <DialogTrigger asChild>
              <Button className="gap-2">
                <PlusCircle className="h-4 w-4" /> Yeni Kategori
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Yeni Kategori</DialogTitle>
                <DialogDescription>Blogunuz için yeni bir kategori oluşturun.</DialogDescription>
              </DialogHeader>
              <form id="create-category-form" onSubmit={onSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="category-name">Kategori Adı</Label>
                  <Input id="category-name" placeholder="Teknoloji" {...formMethods.register('name')} />
                  {formMethods.formState.errors.name && (
                    <p className="text-sm text-destructive">{formMethods.formState.errors.name.message}</p>
                  )}
                </div>
              </form>
              <DialogFooter className="gap-2">
                <Button type="button" variant="ghost" onClick={() => setIsCreateOpen(false)}>
                  İptal
                </Button>
                <Button type="submit" form="create-category-form" disabled={createMutation.isPending}>
                  {createMutation.isPending ? 'Kaydediliyor...' : 'Kaydet'}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
            <Input
              placeholder="Kategori ara..."
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
                {categoriesQuery.isLoading ? (
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
                      Kategori bulunamadı.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>
          <div className="flex flex-col items-center justify-between gap-3 border-t pt-4 text-sm text-muted-foreground md:flex-row">
            <div>
              Toplam {categoriesQuery.data?.count ?? 0} kayıt - Sayfa {currentPage + 1} / {totalPages || 1}
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

      <Dialog open={!!editingCategory} onOpenChange={(open) => !open && setEditingCategory(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Kategoriyi Düzenle</DialogTitle>
            <DialogDescription>Seçili kategorinin adını güncelleyin.</DialogDescription>
          </DialogHeader>
          <form id="edit-category-form" onSubmit={onSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="edit-category-name">Kategori Adı</Label>
              <Input
                id="edit-category-name"
                placeholder="Kategori adı"
                {...formMethods.register('name')}
              />
              {formMethods.formState.errors.name && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.name.message}</p>
              )}
            </div>
          </form>
          <DialogFooter className="gap-2">
            <Button type="button" variant="ghost" onClick={() => setEditingCategory(null)}>
              İptal
            </Button>
            <Button type="submit" form="edit-category-form" disabled={updateMutation.isPending}>
              {updateMutation.isPending ? 'Güncelleniyor...' : 'Güncelle'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={!!categoryToDelete} onOpenChange={(open) => !open && setCategoryToDelete(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Kategoriyi Sil</DialogTitle>
            <DialogDescription>
              Bu kategoriyi silmek istediğinizden emin misiniz? İşlem geri alınamaz.
            </DialogDescription>
          </DialogHeader>
          <Separator />
          <p className="text-sm text-muted-foreground">
            Silinecek kategori: <span className="font-medium">{categoryToDelete?.name}</span>
          </p>
          <DialogFooter className="gap-2">
            <Button type="button" variant="ghost" onClick={() => setCategoryToDelete(null)}>
              İptal
            </Button>
            <Button
              type="button"
              variant="destructive"
              disabled={deleteMutation.isPending}
              onClick={() => categoryToDelete && deleteMutation.mutate(categoryToDelete.id)}
            >
              {deleteMutation.isPending ? 'Siliniyor...' : 'Sil'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
