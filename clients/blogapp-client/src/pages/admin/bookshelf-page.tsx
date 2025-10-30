import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  ColumnDef,
  SortingState,
  flexRender,
  getCoreRowModel,
  useReactTable
} from '@tanstack/react-table';
import { useMutation, useQuery } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { PlusCircle, Pencil, Trash2, ArrowUpDown } from 'lucide-react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';

import {
  fetchBookshelfItems,
  createBookshelfItem,
  updateBookshelfItem,
  deleteBookshelfItem,
  getBookshelfItemById
} from '../../features/bookshelf/api';
import {
  BookshelfFormValues,
  BookshelfItem,
  BookshelfListResponse,
  BookshelfStatusFilter,
  BookshelfTableFilters
} from '../../features/bookshelf/types';
import { bookshelfItemSchema, BookshelfItemFormSchema } from '../../features/bookshelf/schema';
import { useInvalidateQueries } from '../../hooks/use-invalidate-queries';
import { handleApiError, showApiResponseError } from '../../lib/api-error';
import { PermissionGuard } from '../../components/auth/permission-guard';
import { Permissions } from '../../lib/permissions';
import { usePermission } from '../../hooks/use-permission';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Badge } from '../../components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '../../components/ui/dialog';
import { Separator } from '../../components/ui/separator';
import { cn, resolveApiAssetUrl } from '../../lib/utils';
import { ImageUploadField } from '../../components/forms/image-upload-field';

const defaultFilters: BookshelfTableFilters = {
  status: 'all',
  pageIndex: 0,
  pageSize: 10
};

const defaultFormValues: BookshelfItemFormSchema = {
  title: '',
  author: '',
  publisher: '',
  pageCount: '',
  isRead: false,
  notes: '',
  readDate: '',
  imageUrl: '',
  removeImage: false
};

const dateFormatter = new Intl.DateTimeFormat('tr-TR', {
  year: 'numeric',
  month: 'short',
  day: 'numeric'
});

const fieldMap: Record<string, string> = {
  title: 'Title',
  author: 'Author',
  publisher: 'Publisher',
  pageCount: 'PageCount',
  isRead: 'IsRead',
  readDate: 'ReadDate',
  createdDate: 'CreatedDate'
};

export function BookshelfPage() {
  const { hasPermission } = usePermission();
  const { invalidateBookshelf } = useInvalidateQueries();
  const [filters, setFilters] = useState<BookshelfTableFilters>(defaultFilters);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<BookshelfStatusFilter>('all');
  const [sorting, setSorting] = useState<SortingState>([{ id: 'createdDate', desc: true }]);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [formMode, setFormMode] = useState<'create' | 'edit'>('create');
  const [editingItemId, setEditingItemId] = useState<string | null>(null);
  const [isFormLoading, setIsFormLoading] = useState(false);
  const [itemToDelete, setItemToDelete] = useState<BookshelfItem | null>(null);

  const formMethods = useForm<BookshelfItemFormSchema>({
    resolver: zodResolver(bookshelfItemSchema),
    defaultValues: defaultFormValues
  });

  const isReadValue = formMethods.watch('isRead');
  const imageUrlValue = formMethods.watch('imageUrl');
  const titleValue = formMethods.watch('title');

  useEffect(() => {
    formMethods.register('removeImage');
  }, [formMethods]);

  useEffect(() => {
    if (!isReadValue && formMethods.getValues('readDate')) {
      formMethods.setValue('readDate', '', { shouldDirty: true, shouldValidate: true });
    }
  }, [formMethods, isReadValue]);

  const booksQuery = useQuery<BookshelfListResponse>({
    queryKey: [
      'bookshelf-items',
      filters.pageIndex,
      filters.pageSize,
      filters.search ?? '',
      filters.status,
      filters.sort?.field ?? '',
      filters.sort?.dir ?? ''
    ],
    queryFn: () => fetchBookshelfItems(filters),
    placeholderData: (previousData) => previousData
  });

  useEffect(() => {
    const timeout = window.setTimeout(() => {
      setFilters((prev) => ({
        ...prev,
        search: searchTerm.trim(),
        pageIndex: 0
      }));
    }, 400);

    return () => window.clearTimeout(timeout);
  }, [searchTerm]);

  useEffect(() => {
    setFilters((prev) => ({
      ...prev,
      status: statusFilter,
      pageIndex: 0
    }));
  }, [statusFilter]);

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

  const handleEdit = useCallback(async (itemId: string) => {
    setFormMode('edit');
    setEditingItemId(itemId);
    setIsFormLoading(true);

    try {
      const item = await getBookshelfItemById(itemId);
      formMethods.reset({
        title: item.title,
        author: item.author ?? '',
        publisher: item.publisher ?? '',
        pageCount: item.pageCount?.toString() ?? '',
        isRead: item.isRead,
        notes: item.notes ?? '',
        readDate: item.readDate ? new Date(item.readDate).toISOString().slice(0, 10) : '',
        imageUrl: item.imageUrl ?? '',
        removeImage: false
      });
      setIsFormOpen(true);
    } catch (error) {
      handleApiError(error, 'Kitap kaydı yüklenemedi');
      setEditingItemId(null);
    } finally {
      setIsFormLoading(false);
    }
  }, [formMethods]);

  const columns = useMemo<ColumnDef<BookshelfItem>[]>(
    () => [
      {
        accessorKey: 'imageUrl',
        header: 'Görsel',
        cell: ({ row }) => {
          const imageUrl = row.original.imageUrl;
          if (!imageUrl) {
            return <span className="text-xs text-muted-foreground">Yok</span>;
          }

          return (
            <div className="flex h-12 w-12 items-center justify-center overflow-hidden rounded-md border">
              <img
                src={resolveApiAssetUrl(imageUrl)}
                alt={row.original.title}
                className="h-full w-full object-cover"
              />
            </div>
          );
        },
        enableSorting: false,
        size: 80
      },
      {
        accessorKey: 'title',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Kitap
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => <span className="font-medium">{row.original.title}</span>,
        enableSorting: true
      },
      {
        accessorKey: 'author',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Yazar
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => row.original.author ?? '-',
        enableSorting: true
      },
      {
        accessorKey: 'publisher',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Yayınevi
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => row.original.publisher ?? '-',
        enableSorting: true
      },
      {
        accessorKey: 'pageCount',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Sayfa
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => (row.original.pageCount ? row.original.pageCount : '-'),
        enableSorting: true
      },
      {
        accessorKey: 'isRead',
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
          <Badge variant={row.original.isRead ? 'default' : 'secondary'}>
            {row.original.isRead ? 'Okundu' : 'Okunmadı'}
          </Badge>
        ),
        enableSorting: true
      },
      {
        accessorKey: 'readDate',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Okunma Tarihi
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) =>
          row.original.readDate ? dateFormatter.format(new Date(row.original.readDate)) : '-',
        enableSorting: true
      },
      {
        accessorKey: 'createdDate',
        header: ({ column }) => (
          <Button
            variant="ghost"
            className="-ml-3 h-8"
            onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}
          >
            Kaydedilme
            <ArrowUpDown className="ml-2 h-4 w-4" />
          </Button>
        ),
        cell: ({ row }) => dateFormatter.format(new Date(row.original.createdDate)),
        enableSorting: true
      },
      {
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            {hasPermission(Permissions.BookshelfUpdate) && (
              <Button variant="ghost" size="icon" onClick={() => handleEdit(row.original.id)} aria-label="Düzenle">
                <Pencil className="h-4 w-4" />
              </Button>
            )}
            {hasPermission(Permissions.BookshelfDelete) && (
              <Button
                variant="ghost"
                size="icon"
                onClick={() => setItemToDelete(row.original)}
                aria-label="Sil"
              >
                <Trash2 className="h-4 w-4 text-destructive" />
              </Button>
            )}
          </div>
        )
      }
    ],
    [hasPermission, handleEdit]
  );

  const table = useReactTable({
    data: booksQuery.data?.items ?? [],
    columns,
    state: {
      sorting
    },
    onSortingChange: setSorting,
    manualSorting: true,
    getCoreRowModel: getCoreRowModel()
  });

  const createMutation = useMutation({
    mutationFn: (values: BookshelfFormValues) => createBookshelfItem(values),
    onSuccess: (result) => {
      if (!result.success) {
        showApiResponseError(result, 'Kitap kaydı oluşturulamadı');
        return;
      }

      toast.success(result.message || 'Kitap kaydı eklendi');
      closeForm();
      invalidateBookshelf();
    },
    onError: (error) => handleApiError(error, 'Kitap kaydı oluşturulamadı')
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, values }: { id: string; values: BookshelfFormValues }) =>
      updateBookshelfItem(id, values),
    onSuccess: (result) => {
      if (!result.success) {
        showApiResponseError(result, 'Kitap kaydı güncellenemedi');
        return;
      }

      toast.success(result.message || 'Kitap kaydı güncellendi');
      closeForm();
      invalidateBookshelf();
    },
    onError: (error) => handleApiError(error, 'Kitap kaydı güncellenemedi')
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteBookshelfItem(id),
    onSuccess: (result) => {
      if (!result.success) {
        showApiResponseError(result, 'Kitap kaydı silinemedi');
        return;
      }

      toast.success(result.message || 'Kitap kaydı silindi');
      setItemToDelete(null);
      invalidateBookshelf();
    },
    onError: (error) => handleApiError(error, 'Kitap kaydı silinemedi')
  });

  const totalPages = booksQuery.data?.pages ?? 0;
  const currentPage = booksQuery.data?.index ?? filters.pageIndex;
  const isFirstPage = currentPage <= 0;
  const isLastPage = totalPages > 0 ? currentPage >= totalPages - 1 : false;

  function openCreateForm() {
    setFormMode('create');
    setEditingItemId(null);
    formMethods.reset(defaultFormValues);
    setIsFormOpen(true);
  }

  function closeForm() {
    setIsFormOpen(false);
    setEditingItemId(null);
    formMethods.reset(defaultFormValues);
  }

  const onSubmit = formMethods.handleSubmit((data) => {
    const payload: BookshelfFormValues = {
      title: data.title,
      author: data.author ?? '',
      publisher: data.publisher ?? '',
      pageCount: data.pageCount ?? '',
      isRead: data.isRead,
      notes: data.notes ?? '',
      readDate: data.readDate ?? '',
      imageUrl: data.imageUrl ?? '',
      removeImage: data.removeImage ?? false
    };

    if (formMode === 'edit' && editingItemId) {
      updateMutation.mutate({ id: editingItemId, values: payload });
    } else {
      createMutation.mutate(payload);
    }
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

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <CardTitle>Kitaplık</CardTitle>
            <p className="text-sm text-muted-foreground">
              Okuduğunuz veya okumak istediğiniz kitapları yönetin. Arama, filtreleme ve sıralama seçeneklerini kullanın.
            </p>
          </div>
          <PermissionGuard requiredPermission={Permissions.BookshelfCreate}>
            <Button className="gap-2" onClick={openCreateForm}>
              <PlusCircle className="h-4 w-4" /> Yeni Kayıt
            </Button>
          </PermissionGuard>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-[1fr_auto_auto] md:items-center">
            <Input
              placeholder="Kitap, yazar veya yayınevi ara..."
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
            />
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <span>Durum:</span>
              <select
                className="rounded-md border bg-background px-2 py-1"
                value={statusFilter}
                onChange={(event) => setStatusFilter(event.target.value as BookshelfStatusFilter)}
              >
                <option value="all">Tümü</option>
                <option value="read">Okunanlar</option>
                <option value="unread">Okunacaklar</option>
              </select>
            </div>
            <div className="flex items-center gap-2 justify-self-start text-sm text-muted-foreground">
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
                      <TableHead key={header.id} className={cn(header.column.id === 'actions' && 'w-[140px]')}>
                        {header.isPlaceholder
                          ? null
                          : flexRender(header.column.columnDef.header, header.getContext())}
                      </TableHead>
                    ))}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {booksQuery.isLoading ? (
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
                      Kayıt bulunamadı.
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>

          <div className="flex flex-col items-center justify-between gap-3 border-t pt-4 text-sm text-muted-foreground md:flex-row">
            <div>
              Toplam {booksQuery.data?.count ?? 0} kayıt - Sayfa {currentPage + 1} / {totalPages || 1}
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

      <Dialog open={isFormOpen} onOpenChange={(open) => !open && closeForm()}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{formMode === 'edit' ? 'Kitap Kaydını Düzenle' : 'Yeni Kitap Kaydı'}</DialogTitle>
            <DialogDescription>
              {formMode === 'edit'
                ? 'Seçili kitap kaydını güncelleyin.'
                : 'Okuma listenize yeni bir kitap ekleyin.'}
            </DialogDescription>
          </DialogHeader>
          <form id="bookshelf-form" onSubmit={onSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="book-title">Kitap Adı</Label>
              <Input id="book-title" placeholder="Kitap adı" {...formMethods.register('title')} disabled={isFormLoading} />
              {formMethods.formState.errors.title && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.title.message}</p>
              )}
            </div>
            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="book-author">Yazar</Label>
                <Input id="book-author" placeholder="Yazar" {...formMethods.register('author')} disabled={isFormLoading} />
                {formMethods.formState.errors.author && (
                  <p className="text-sm text-destructive">{formMethods.formState.errors.author.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="book-publisher">Yayınevi</Label>
                <Input id="book-publisher" placeholder="Yayınevi" {...formMethods.register('publisher')} disabled={isFormLoading} />
                {formMethods.formState.errors.publisher && (
                  <p className="text-sm text-destructive">{formMethods.formState.errors.publisher.message}</p>
                )}
              </div>
            </div>
            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="book-pageCount">Sayfa Sayısı</Label>
                <Input
                  id="book-pageCount"
                  type="number"
                  min={1}
                  max={20000}
                  placeholder="Örn. 320"
                  {...formMethods.register('pageCount')}
                  disabled={isFormLoading}
                />
                {formMethods.formState.errors.pageCount && (
                  <p className="text-sm text-destructive">{formMethods.formState.errors.pageCount.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="book-readDate">Okunma Tarihi</Label>
                <Input
                  id="book-readDate"
                  type="date"
                  {...formMethods.register('readDate')}
                  disabled={isFormLoading || !formMethods.watch('isRead')}
                />
                {formMethods.formState.errors.readDate && (
                  <p className="text-sm text-destructive">{formMethods.formState.errors.readDate.message}</p>
                )}
              </div>
            </div>
            <div className="flex items-center gap-3">
              <input
                id="book-isRead"
                type="checkbox"
                className="h-4 w-4"
                {...formMethods.register('isRead')}
                disabled={isFormLoading}
              />
              <Label htmlFor="book-isRead" className="cursor-pointer">
                Bu kitabı okudum
              </Label>
            </div>
            <div className="space-y-2">
              <ImageUploadField
                label="Kapak Görseli"
                description="Kitap için kapak görseli yükleyin. JPG, PNG veya WEBP formatlarını kullanabilirsiniz."
                value={imageUrlValue}
                title={titleValue}
                onChange={(url) => {
                  formMethods.setValue('imageUrl', url, { shouldDirty: true });
                  formMethods.setValue('removeImage', false, { shouldDirty: true });
                }}
                onRemove={() => {
                  formMethods.setValue('imageUrl', '', { shouldDirty: true });
                  formMethods.setValue('removeImage', true, { shouldDirty: true });
                }}
                isDisabled={isFormLoading || isSubmitting}
                scope="bookshelf"
                resizeMode="fit"
                maxWidth={1200}
                maxHeight={1200}
              />
              {formMethods.formState.errors.imageUrl && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.imageUrl.message}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="book-notes">Not</Label>
              <textarea
                id="book-notes"
                rows={4}
                className="flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                placeholder="Kitapla ilgili notlarınızı girin"
                {...formMethods.register('notes')}
                disabled={isFormLoading}
              />
              {formMethods.formState.errors.notes && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.notes.message}</p>
              )}
            </div>
          </form>
          <DialogFooter className="gap-2">
            <Button type="button" variant="ghost" onClick={closeForm} disabled={isSubmitting}>
              İptal
            </Button>
            <Button type="submit" form="bookshelf-form" disabled={isSubmitting || isFormLoading}>
              {isSubmitting ? 'Kaydediliyor...' : formMode === 'edit' ? 'Güncelle' : 'Kaydet'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={!!itemToDelete} onOpenChange={(open) => !open && setItemToDelete(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Kitap Kaydını Sil</DialogTitle>
            <DialogDescription>Bu kayıt kalıcı olarak silinecek. Devam etmek istediğinize emin misiniz?</DialogDescription>
          </DialogHeader>
          <Separator />
          <p className="text-sm text-muted-foreground">
            Silinecek kitap: <span className="font-medium">{itemToDelete?.title}</span>
          </p>
          <DialogFooter className="gap-2">
            <Button type="button" variant="ghost" onClick={() => setItemToDelete(null)}>
              İptal
            </Button>
            <Button
              type="button"
              variant="destructive"
              disabled={deleteMutation.isPending}
              onClick={() => itemToDelete && deleteMutation.mutate(itemToDelete.id)}
            >
              {deleteMutation.isPending ? 'Siliniyor...' : 'Sil'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
