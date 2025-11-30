import { useEffect, useMemo, useState } from 'react';
import {
  ColumnDef,
  SortingState,
  flexRender,
  getCoreRowModel,
  useReactTable
} from '@tanstack/react-table';
import { useMutation, useQuery } from '@tanstack/react-query';
import { PlusCircle, Pencil, Trash2, ArrowUpDown, ChevronRight, ChevronDown, FolderTree } from 'lucide-react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { createCategory, updateCategory, deleteCategory, getAllCategories } from '../../features/categories/api';
import {
  Category,
  CategoryFormValues
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
import { Badge } from '../../components/ui/badge';
import { useInvalidateQueries } from '../../hooks/use-invalidate-queries';
import toast from 'react-hot-toast';
import { handleApiError, showApiResponseError } from '../../lib/api-error';
import { cn } from '../../lib/utils';
import { PermissionGuard } from '../../components/auth/permission-guard';
import { Permissions } from '../../lib/permissions';
import { usePermission } from '../../hooks/use-permission';

const categorySchema = z.object({
  name: z.string().min(5, 'Kategori adı en az 5 karakter olmalıdır').max(100, 'Kategori adı en fazla 100 karakter olabilir'),
  description: z.string().max(500, 'Açıklama en fazla 500 karakter olabilir').optional(),
  parentId: z.string().uuid('Geçerli bir üst kategori seçin').optional().or(z.literal(''))
});

type CategoryFormSchema = z.infer<typeof categorySchema>;

export function CategoriesPage() {
  const { hasPermission } = usePermission();
  const { invalidateCategories } = useInvalidateQueries();
  const [searchTerm, setSearchTerm] = useState('');
  const [sorting, setSorting] = useState<SortingState>([
    { id: 'createdDate', desc: true }
  ]);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [categoryToDelete, setCategoryToDelete] = useState<Category | null>(null);
  const [expandedParents, setExpandedParents] = useState<Set<string>>(new Set());

  // Tüm kategorileri yükle (parent-child yapısı için)
  const parentCategoriesQuery = useQuery({
    queryKey: ['all-categories'],
    queryFn: getAllCategories
  });


  // Kategorileri parent-child yapısına göre organize et
  const organizedCategories = useMemo(() => {
    if (!parentCategoriesQuery.data) {
      return { parents: [], childrenMap: new Map<string, Category[]>() };
    }
    
    const parents: Category[] = [];
    const childrenMap = new Map<string, Category[]>();
    
    parentCategoriesQuery.data.forEach((category) => {
      if (category.parentId) {
        if (!childrenMap.has(category.parentId)) {
          childrenMap.set(category.parentId, []);
        }
        childrenMap.get(category.parentId)!.push(category);
      } else {
        parents.push(category);
      }
    });
    
    return { parents, childrenMap };
  }, [parentCategoriesQuery.data]);

  // Expand/collapse toggle fonksiyonu
  const toggleExpand = (parentId: string) => {
    setExpandedParents((prev) => {
      const next = new Set(prev);
      if (next.has(parentId)) {
        next.delete(parentId);
      } else {
        next.add(parentId);
      }
      return next;
    });
  };

  // Tree view için render edilecek kategorileri oluştur
  const treeViewCategories = useMemo(() => {
    const result: (Category & { isChild: boolean; level: number })[] = [];
    const searchLower = searchTerm.toLowerCase();
    
    organizedCategories.parents.forEach((parent) => {
      // Arama filtresi: Parent veya child'lardan biri arama terimini içeriyorsa göster
      const parentMatches = !searchTerm || 
        parent.name.toLowerCase().includes(searchLower) ||
        parent.description?.toLowerCase().includes(searchLower);
      
      const children = organizedCategories.childrenMap.get(parent.id) || [];
      const visibleChildren = children.filter(child => 
        !searchTerm || 
        child.name.toLowerCase().includes(searchLower) ||
        child.description?.toLowerCase().includes(searchLower) ||
        parent.name.toLowerCase().includes(searchLower)
      );
      
      const hasVisibleChildren = visibleChildren.length > 0;
      const shouldShowParent = parentMatches || hasVisibleChildren || expandedParents.has(parent.id);
      
      if (shouldShowParent) {
        result.push({ ...parent, isChild: false, level: 0 });
        
        // Eğer arama yapılıyorsa veya parent açıksa child'ları göster
        const shouldShowChildren = searchTerm 
          ? hasVisibleChildren 
          : expandedParents.has(parent.id);
          
        if (shouldShowChildren) {
          visibleChildren.forEach((child) => {
            result.push({ ...child, isChild: true, level: 1 });
          });
        }
      }
    });
    
    return result;
  }, [organizedCategories, expandedParents, searchTerm]);

  const columns = useMemo<ColumnDef<Category>[]>(
    () => [
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
        cell: ({ row }) => {
          const category = row.original;
          const isChild = !!category.parentId;
          const hasChildren = organizedCategories.childrenMap.has(category.id);
          const isExpanded = expandedParents.has(category.id);
          
          return (
            <div className="flex items-center gap-3">
              {isChild ? (
                <div className="flex items-center gap-3 w-full pl-8">
                  <div className="flex items-center gap-2 flex-1 min-w-0">
                    <div className="h-6 w-px bg-border mr-2" />
                    <div className="flex items-center gap-2 min-w-0">
                      <span className="text-sm font-medium truncate">{category.name}</span>
                      <Badge variant="outline" className="text-xs px-1.5 py-0 h-5 shrink-0">
                        Alt Kategori
                      </Badge>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="flex items-center gap-2 w-full">
                  {hasChildren ? (
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-6 w-6 shrink-0"
                      onClick={(e) => {
                        e.stopPropagation();
                        toggleExpand(category.id);
                      }}
                      aria-label={isExpanded ? 'Kapat' : 'Aç'}
                    >
                      {isExpanded ? (
                        <ChevronDown className="h-4 w-4" />
                      ) : (
                        <ChevronRight className="h-4 w-4" />
                      )}
                    </Button>
                  ) : (
                    <div className="w-6" />
                  )}
                  <FolderTree className="h-5 w-5 text-primary shrink-0" />
                  <div className="flex items-center gap-2">
                    <span className="font-semibold text-base">{category.name}</span>
                    <Badge variant="secondary" className="text-xs">
                      Ana Kategori
                    </Badge>
                    {hasChildren && (
                      <Badge variant="outline" className="text-xs">
                        {organizedCategories.childrenMap.get(category.id)?.length || 0} alt kategori
                      </Badge>
                    )}
                  </div>
                </div>
              )}
            </div>
          );
        },
        enableSorting: true
      },
      {
        accessorKey: 'description',
        header: 'Açıklama',
        cell: ({ row }) => (
          <span className="text-sm text-muted-foreground">
            {row.original.description || '-'}
          </span>
        )
      },
      {
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            {hasPermission(Permissions.CategoriesUpdate) && (
              <Button
                variant="ghost"
                size="icon"
                onClick={() => setEditingCategory(row.original)}
                aria-label="Düzenle"
              >
                <Pencil className="h-4 w-4" />
              </Button>
            )}
            {hasPermission(Permissions.CategoriesDelete) && (
              <Button
                variant="ghost"
                size="icon"
                onClick={() => setCategoryToDelete(row.original)}
                aria-label="Sil"
              >
                <Trash2 className="h-4 w-4 text-destructive" />
              </Button>
            )}
          </div>
        )
      }
    ],
    [hasPermission, organizedCategories, expandedParents]
  );

  const table = useReactTable({
    data: treeViewCategories,
    columns,
    state: {
      sorting
    },
    onSortingChange: setSorting,
    manualSorting: false,
    getCoreRowModel: getCoreRowModel()
  });

  const createMutation = useMutation({
    mutationFn: createCategory,
    onSuccess: (result) => {
      if (!result.success) {
        showApiResponseError(result, 'Kategori eklenemedi');
        return;
      }
      toast.success(result.message || 'Kategori eklendi');
      setIsCreateOpen(false);
      invalidateCategories();
    },
    onError: (error) => handleApiError(error, 'Kategori eklenemedi')
  });

  const updateMutation = useMutation({
    mutationFn: (values: CategoryFormValues) =>
      editingCategory ? updateCategory(editingCategory.id, values) : Promise.reject(),
    onSuccess: (result) => {
      if (!result.success) {
        showApiResponseError(result, 'Kategori güncellenemedi');
        return;
      }
      toast.success(result.message || 'Kategori güncellendi');
      setEditingCategory(null);
      invalidateCategories();
    },
    onError: (error) => handleApiError(error, 'Kategori güncellenemedi')
  });

  const deleteMutation = useMutation({
    mutationFn: (categoryId: string) => deleteCategory(categoryId),
    onSuccess: (result) => {
      if (!result.success) {
        showApiResponseError(result, 'Kategori silinemedi');
        return;
      }
      toast.success(result.message || 'Kategori silindi');
      setCategoryToDelete(null);
      invalidateCategories();
    },
    onError: (error) => handleApiError(error, 'Kategori silinemedi')
  });


  const formMethods = useForm<CategoryFormSchema>({
    resolver: zodResolver(categorySchema),
    defaultValues: {
      name: '',
      description: '',
      parentId: ''
    }
  });

  useEffect(() => {
    if (editingCategory) {
      formMethods.reset({
        name: editingCategory.name,
        description: editingCategory.description || '',
        parentId: editingCategory.parentId || ''
      });
    } else {
      formMethods.reset({ name: '', description: '', parentId: '' });
    }
  }, [editingCategory, formMethods]);

  // Düzenleme sırasında mevcut kategoriyi parent listesinden çıkar
  const availableParentCategories = useMemo(() => {
    if (!parentCategoriesQuery.data) return [];
    if (!editingCategory) return parentCategoriesQuery.data;
    // Düzenlenen kategoriyi ve alt kategorilerini parent olarak seçilemez yap
    return parentCategoriesQuery.data.filter(cat => cat.id !== editingCategory.id);
  }, [parentCategoriesQuery.data, editingCategory]);

  const onSubmit = formMethods.handleSubmit(async (values) => {
    const formData: CategoryFormValues = {
      name: values.name,
      description: values.description || undefined,
      parentId: values.parentId && values.parentId !== '' ? values.parentId : undefined
    };
    if (editingCategory) {
      await updateMutation.mutateAsync(formData);
    } else {
      await createMutation.mutateAsync(formData);
    }
    formMethods.reset({ name: '', description: '', parentId: '' });
  });


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
          <PermissionGuard requiredPermission={Permissions.CategoriesCreate}>
            <Dialog
              open={isCreateOpen}
              onOpenChange={(open) => {
                setIsCreateOpen(open);
                if (!open && !editingCategory) {
                  formMethods.reset({ name: '', description: '', parentId: '' });
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
                  <Label htmlFor="category-name">Kategori Adı *</Label>
                  <Input id="category-name" placeholder="Teknoloji" {...formMethods.register('name')} />
                  {formMethods.formState.errors.name && (
                    <p className="text-sm text-destructive">{formMethods.formState.errors.name.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor="category-description">Açıklama</Label>
                  <textarea
                    id="category-description"
                    placeholder="Kategori açıklaması..."
                    className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                    {...formMethods.register('description')}
                  />
                  {formMethods.formState.errors.description && (
                    <p className="text-sm text-destructive">{formMethods.formState.errors.description.message}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor="category-parent">Üst Kategori</Label>
                  <select
                    id="category-parent"
                    className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                    {...formMethods.register('parentId')}
                  >
                    <option value="">Üst kategori yok</option>
                    {parentCategoriesQuery.data?.map((cat) => (
                      <option key={cat.id} value={cat.id}>
                        {cat.name}
                      </option>
                    ))}
                  </select>
                  {formMethods.formState.errors.parentId && (
                    <p className="text-sm text-destructive">{formMethods.formState.errors.parentId.message}</p>
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
          </PermissionGuard>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
            <Input
              placeholder="Kategori ara..."
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              className="md:w-72"
            />
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
                {parentCategoriesQuery.isLoading ? (
                  <TableRow>
                    <TableCell colSpan={columns.length} className="h-24 text-center">
                      Veriler yükleniyor...
                    </TableCell>
                  </TableRow>
                ) : table.getRowModel().rows.length ? (
                  table.getRowModel().rows.map((row) => {
                    const category = row.original;
                    const isChild = !!category.parentId;
                    
                    return (
                      <TableRow 
                        key={row.id}
                        className={cn(
                          isChild 
                            ? 'bg-muted/30 hover:bg-muted/50' 
                            : 'bg-background hover:bg-muted/30 border-l-2 border-l-primary'
                        )}
                      >
                        {row.getVisibleCells().map((cell) => (
                          <TableCell 
                            key={cell.id}
                            className={cn(isChild && 'pl-8')}
                          >
                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                          </TableCell>
                        ))}
                      </TableRow>
                    );
                  })
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
              Toplam {organizedCategories.parents.length} ana kategori, {treeViewCategories.filter(c => c.isChild).length} alt kategori
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
              <Label htmlFor="edit-category-name">Kategori Adı *</Label>
              <Input
                id="edit-category-name"
                placeholder="Kategori adı"
                {...formMethods.register('name')}
              />
              {formMethods.formState.errors.name && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.name.message}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-category-description">Açıklama</Label>
              <textarea
                id="edit-category-description"
                placeholder="Kategori açıklaması..."
                className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                {...formMethods.register('description')}
              />
              {formMethods.formState.errors.description && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.description.message}</p>
              )}
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-category-parent">Üst Kategori</Label>
              <select
                id="edit-category-parent"
                className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                {...formMethods.register('parentId')}
              >
                <option value="">Üst kategori yok</option>
                {availableParentCategories.map((cat) => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
              {formMethods.formState.errors.parentId && (
                <p className="text-sm text-destructive">{formMethods.formState.errors.parentId.message}</p>
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
