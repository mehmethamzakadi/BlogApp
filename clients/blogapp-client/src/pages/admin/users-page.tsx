import { useEffect, useMemo, useState } from 'react';
import {
  ColumnDef,
  SortingState,
  flexRender,
  getCoreRowModel,
  useReactTable
} from '@tanstack/react-table';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { PlusCircle, Pencil, Trash2, ArrowUpDown, Shield, Mail, User as UserIcon } from 'lucide-react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import {
  fetchUsers,
  createUser,
  updateUser,
  deleteUser,
  fetchUserRoles,
  assignRolesToUser
} from '../../features/users/api';
import { fetchRoles } from '../../features/roles/api';
import {
  User,
  UserFormValues,
  UserUpdateFormValues,
  UserTableFilters,
  UserListResponse
} from '../../features/users/types';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Checkbox } from '../../components/ui/checkbox';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '../../components/ui/dialog';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { useInvalidateQueries } from '../../hooks/use-invalidate-queries';
import toast from 'react-hot-toast';
import { PermissionGuard } from '../../components/auth/permission-guard';
import { Permissions } from '../../lib/permissions';
import { usePermission } from '../../hooks/use-permission';

const userCreateSchema = z.object({
  userName: z.string().min(3, 'Kullanıcı adı en az 3 karakter olmalıdır'),
  email: z.string().email('Geçerli bir email adresi giriniz'),
  password: z.string().min(6, 'Şifre en az 6 karakter olmalıdır')
});

const userUpdateSchema = z.object({
  userName: z.string().min(3, 'Kullanıcı adı en az 3 karakter olmalıdır'),
  email: z.string().email('Geçerli bir email adresi giriniz')
});

type UserCreateFormSchema = z.infer<typeof userCreateSchema>;
type UserUpdateFormSchema = z.infer<typeof userUpdateSchema>;

const fieldMap: Record<string, string> = {
  id: 'Id',
  firstName: 'FirstName',
  lastName: 'LastName',
  userName: 'UserName',
  email: 'Email'
};

export function UsersPage() {
  const queryClient = useQueryClient();
  const { hasPermission } = usePermission();
  const { invalidateUsers } = useInvalidateQueries();
  const [filters, setFilters] = useState<UserTableFilters>({
    pageIndex: 0,
    pageSize: 10
  });
  const [searchTerm, setSearchTerm] = useState('');
  const [sorting, setSorting] = useState<SortingState>([{ id: 'id', desc: true }]);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [userToDelete, setUserToDelete] = useState<User | null>(null);
  const [assigningRolesUser, setAssigningRolesUser] = useState<User | null>(null);
  const [selectedRoles, setSelectedRoles] = useState<number[]>([]);

  // Queries
  const usersQuery = useQuery<UserListResponse>({
    queryKey: [
      'users',
      filters.pageIndex,
      filters.pageSize,
      filters.search ?? '',
      filters.sort?.field ?? '',
      filters.sort?.dir ?? ''
    ],
    queryFn: () => fetchUsers(filters),
    placeholderData: (previousData) => previousData
  });

  const rolesQuery = useQuery({
    queryKey: ['roles'],
    queryFn: () => fetchRoles(0, 100)
  });

  const userRolesQuery = useQuery({
    queryKey: ['user-roles', assigningRolesUser?.id],
    queryFn: () => fetchUserRoles(assigningRolesUser!.id),
    enabled: !!assigningRolesUser
  });

  // Effects
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

      return { ...prev, sort: nextSort, pageIndex: 0 };
    });
  }, [sorting]);

  useEffect(() => {
    if (assigningRolesUser) {
      // When dialog opens, set selectedRoles from query data if available
      if (userRolesQuery.data?.roles) {
        const roleIds = userRolesQuery.data.roles.map(role => role.id);
        setSelectedRoles(roleIds);
      }
    } else {
      // Reset selectedRoles when dialog is closed
      setSelectedRoles([]);
    }
  }, [assigningRolesUser, userRolesQuery.data]);

  // Columns
  const columns = useMemo<ColumnDef<User>[]>(
    () => [
      {
        accessorKey: 'id',
        header: 'ID',
        cell: ({ row }) => <span className="font-mono text-xs text-muted-foreground">#{row.original.id}</span>,
        enableSorting: true
      },
      {
        accessorKey: 'userName',
        header: 'Kullanıcı Adı',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            <UserIcon className="h-4 w-4 text-muted-foreground" />
            <span>{row.original.userName}</span>
          </div>
        )
      },
      {
        accessorKey: 'email',
        header: 'Email',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            <Mail className="h-4 w-4 text-muted-foreground" />
            <span className="text-sm">{row.original.email}</span>
          </div>
        )
      },
      {
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            {hasPermission(Permissions.RolesAssignPermissions) && (
              <Button
                variant="ghost"
                size="icon"
                onClick={() => setAssigningRolesUser(row.original)}
                aria-label="Rol Ata"
                title="Rol Ata"
              >
                <Shield className="h-4 w-4" />
              </Button>
            )}
            {hasPermission(Permissions.UsersUpdate) && (
              <Button
                variant="ghost"
                size="icon"
                onClick={() => setEditingUser(row.original)}
                aria-label="Düzenle"
              >
                <Pencil className="h-4 w-4" />
              </Button>
            )}
            {hasPermission(Permissions.UsersDelete) && (
              <Button
                variant="ghost"
                size="icon"
                onClick={() => setUserToDelete(row.original)}
                aria-label="Sil"
              >
                <Trash2 className="h-4 w-4 text-destructive" />
              </Button>
            )}
          </div>
        )
      }
    ],
    []
  );

  const table = useReactTable({
    data: usersQuery.data?.items ?? [],
    columns,
    state: { sorting },
    onSortingChange: setSorting,
    manualSorting: true,
    getCoreRowModel: getCoreRowModel()
  });

  // Mutations
  const createMutation = useMutation({
    mutationFn: createUser,
    onSuccess: () => {
      toast.success('Kullanıcı başarıyla oluşturuldu');
      setIsCreateOpen(false);
      invalidateUsers();
      createForm.reset();
    },
    onError: () => {
      toast.error('Kullanıcı oluşturulurken hata oluştu');
    }
  });

  const updateMutation = useMutation({
    mutationFn: (values: UserUpdateFormSchema) =>
      editingUser ? updateUser({ ...values, id: editingUser.id }) : Promise.reject(),
    onSuccess: () => {
      toast.success('Kullanıcı başarıyla güncellendi');
      setEditingUser(null);
      invalidateUsers();
    },
    onError: () => {
      toast.error('Kullanıcı güncellenirken hata oluştu');
    }
  });

  const deleteMutation = useMutation({
    mutationFn: (userId: number) => deleteUser(userId),
    onSuccess: () => {
      toast.success('Kullanıcı başarıyla silindi');
      setUserToDelete(null);
      invalidateUsers();
    },
    onError: () => {
      toast.error('Kullanıcı silinirken hata oluştu');
    }
  });

  const assignRolesMutation = useMutation({
    mutationFn: (data: { userId: number; roleIds: number[] }) =>
      assignRolesToUser(data),
    onSuccess: () => {
      toast.success('Roller başarıyla atandı');
      setAssigningRolesUser(null);
      invalidateUsers();
    },
    onError: () => {
      toast.error('Roller atanırken hata oluştu');
    }
  });

  // Forms
  const createForm = useForm<UserCreateFormSchema>({
    resolver: zodResolver(userCreateSchema),
    defaultValues: {
      userName: '',
      email: '',
      password: ''
    }
  });

  const updateForm = useForm<UserUpdateFormSchema>({
    resolver: zodResolver(userUpdateSchema),
    defaultValues: {
      userName: '',
      email: ''
    }
  });

  useEffect(() => {
    if (editingUser) {
      updateForm.reset({
        userName: editingUser.userName,
        email: editingUser.email
      });
    } else {
      updateForm.reset();
    }
  }, [editingUser, updateForm]);

  // Handlers
  const handlePageChange = (direction: 'prev' | 'next') => {
    setFilters((prev) => {
      const nextIndex = direction === 'prev' ? Math.max(prev.pageIndex - 1, 0) : prev.pageIndex + 1;
      return { ...prev, pageIndex: nextIndex };
    });
  };

  const handlePageSizeChange = (value: number) => {
    setFilters((prev) => ({ ...prev, pageSize: value, pageIndex: 0 }));
  };

  const onCreateSubmit = (values: UserCreateFormSchema) => {
    createMutation.mutate(values);
  };

  const onUpdateSubmit = (values: UserUpdateFormSchema) => {
    updateMutation.mutate(values);
  };

  const handleAssignRoles = () => {
    if (!assigningRolesUser) return;
    assignRolesMutation.mutate({
      userId: assigningRolesUser.id,
      roleIds: selectedRoles
    });
  };

  const toggleRole = (roleId: number) => {
    setSelectedRoles((prev) => {
      const currentRoles = prev ?? [];
      return currentRoles.includes(roleId) 
        ? currentRoles.filter((id) => id !== roleId) 
        : [...currentRoles, roleId];
    });
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Kullanıcı Yönetimi</h1>
          <p className="text-muted-foreground mt-2">Sistemdeki kullanıcıları yönetin</p>
        </div>
        <PermissionGuard requiredPermission={Permissions.UsersCreate}>
          <Button onClick={() => setIsCreateOpen(true)}>
            <PlusCircle className="w-4 h-4 mr-2" />
            Yeni Kullanıcı
          </Button>
        </PermissionGuard>
      </div>

      {/* Search & Table */}
      <Card>
        <CardHeader>
          <CardTitle>Kullanıcılar</CardTitle>
          <div className="flex items-center gap-4 mt-4">
            <Input
              placeholder="Ad, soyad, email veya kullanıcı adı ara..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="max-w-md"
            />
          </div>
        </CardHeader>
        <CardContent>
          <div className="rounded-md border">
            <Table>
              <TableHeader>
                {table.getHeaderGroups().map((headerGroup) => (
                  <TableRow key={headerGroup.id}>
                    {headerGroup.headers.map((header) => (
                      <TableHead key={header.id}>
                        {header.isPlaceholder
                          ? null
                          : flexRender(header.column.columnDef.header, header.getContext())}
                      </TableHead>
                    ))}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {table.getRowModel().rows?.length ? (
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
                      {usersQuery.isLoading ? 'Yükleniyor...' : 'Kullanıcı bulunamadı.'}
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>

          {/* Pagination */}
          <div className="flex items-center justify-between mt-4">
            <div className="flex items-center gap-2">
              <span className="text-sm text-muted-foreground">Sayfa başına:</span>
              <select
                value={filters.pageSize}
                onChange={(e) => handlePageSizeChange(Number(e.target.value))}
                className="border rounded px-2 py-1"
              >
                <option value={10}>10</option>
                <option value={20}>20</option>
                <option value={50}>50</option>
              </select>
            </div>
            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => handlePageChange('prev')}
                disabled={!usersQuery.data?.hasPrevious}
              >
                Önceki
              </Button>
              <span className="text-sm">
                Sayfa {(usersQuery.data?.index ?? 0) + 1} / {usersQuery.data?.pages ?? 1}
              </span>
              <Button
                variant="outline"
                size="sm"
                onClick={() => handlePageChange('next')}
                disabled={!usersQuery.data?.hasNext}
              >
                Sonraki
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Create User Dialog */}
      <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Yeni Kullanıcı Ekle</DialogTitle>
            <DialogDescription>Sisteme yeni kullanıcı ekleyin</DialogDescription>
          </DialogHeader>
          <form onSubmit={createForm.handleSubmit(onCreateSubmit)} className="space-y-4">
            <div>
              <Label htmlFor="userName">Kullanıcı Adı</Label>
              <Input id="userName" {...createForm.register('userName')} />
              {createForm.formState.errors.userName && (
                <p className="text-sm text-destructive mt-1">
                  {createForm.formState.errors.userName.message}
                </p>
              )}
            </div>
            <div>
              <Label htmlFor="email">Email</Label>
              <Input id="email" type="email" {...createForm.register('email')} />
              {createForm.formState.errors.email && (
                <p className="text-sm text-destructive mt-1">
                  {createForm.formState.errors.email.message}
                </p>
              )}
            </div>
            <div>
              <Label htmlFor="password">Şifre</Label>
              <Input id="password" type="password" {...createForm.register('password')} />
              {createForm.formState.errors.password && (
                <p className="text-sm text-destructive mt-1">
                  {createForm.formState.errors.password.message}
                </p>
              )}
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>
                İptal
              </Button>
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? 'Ekleniyor...' : 'Ekle'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* Update User Dialog */}
      <Dialog open={!!editingUser} onOpenChange={() => setEditingUser(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Kullanıcı Düzenle</DialogTitle>
            <DialogDescription>Kullanıcı bilgilerini güncelleyin</DialogDescription>
          </DialogHeader>
          <form onSubmit={updateForm.handleSubmit(onUpdateSubmit)} className="space-y-4">
            <div>
              <Label htmlFor="edit-userName">Kullanıcı Adı</Label>
              <Input id="edit-userName" {...updateForm.register('userName')} />
              {updateForm.formState.errors.userName && (
                <p className="text-sm text-destructive mt-1">
                  {updateForm.formState.errors.userName.message}
                </p>
              )}
            </div>
            <div>
              <Label htmlFor="edit-email">Email</Label>
              <Input id="edit-email" type="email" {...updateForm.register('email')} />
              {updateForm.formState.errors.email && (
                <p className="text-sm text-destructive mt-1">
                  {updateForm.formState.errors.email.message}
                </p>
              )}
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setEditingUser(null)}>
                İptal
              </Button>
              <Button type="submit" disabled={updateMutation.isPending}>
                {updateMutation.isPending ? 'Güncelleniyor...' : 'Güncelle'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={!!userToDelete} onOpenChange={() => setUserToDelete(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Kullanıcı Sil</DialogTitle>
            <DialogDescription>
              <strong>{userToDelete?.userName}</strong> kullanıcısını silmek istediğinizden emin misiniz?
              Bu işlem geri alınamaz.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setUserToDelete(null)}>
              İptal
            </Button>
            <Button
              variant="destructive"
              onClick={() => userToDelete && deleteMutation.mutate(userToDelete.id)}
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? 'Siliniyor...' : 'Sil'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Assign Roles Dialog */}
      <Dialog open={!!assigningRolesUser} onOpenChange={() => setAssigningRolesUser(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Rol Ata</DialogTitle>
            <DialogDescription>
              <strong>{assigningRolesUser?.userName}</strong> kullanıcısına roller atayın
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            {(rolesQuery.isLoading || userRolesQuery.isLoading) && (
              <p className="text-sm text-muted-foreground">Yükleniyor...</p>
            )}
            {!rolesQuery.isLoading && !userRolesQuery.isLoading && rolesQuery.data?.items.map((role) => (
              <div key={role.id} className="flex items-center space-x-2">
                <Checkbox
                  id={`role-${role.id}`}
                  checked={selectedRoles.includes(role.id)}
                  onCheckedChange={() => toggleRole(role.id)}
                />
                <label
                  htmlFor={`role-${role.id}`}
                  className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                >
                  {role.name}
                </label>
              </div>
            ))}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setAssigningRolesUser(null)}>
              İptal
            </Button>
            <Button onClick={handleAssignRoles} disabled={assignRolesMutation.isPending}>
              {assignRolesMutation.isPending ? 'Atanıyor...' : 'Kaydet'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
