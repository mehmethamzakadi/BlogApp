import { useEffect, useMemo, useState } from 'react';
import { ColumnDef, flexRender, getCoreRowModel, useReactTable } from '@tanstack/react-table';
import { useMutation, useQuery } from '@tanstack/react-query';
import { PlusCircle, Pencil, Trash2, Shield } from 'lucide-react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { fetchRoles, createRole, updateRole, deleteRole } from '../../features/roles/api';
import { fetchAllPermissions, fetchRolePermissions, assignPermissionsToRole } from '../../features/permissions/api';
import { Role } from '../../features/roles/types';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../../components/ui/table';
import { Checkbox } from '../../components/ui/checkbox';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '../../components/ui/dialog';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { useInvalidateQueries } from '../../hooks/use-invalidate-queries';
import toast from 'react-hot-toast';

const roleSchema = z.object({
  name: z.string().min(2, 'Rol adı en az 2 karakter olmalıdır')
});

type RoleFormSchema = z.infer<typeof roleSchema>;

export function RolesPage() {
  const { invalidateRoles } = useInvalidateQueries();
  const [page, setPage] = useState(0);
  const pageSize = 10;
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [roleToDelete, setRoleToDelete] = useState<Role | null>(null);
  const [assigningPermissionsRole, setAssigningPermissionsRole] = useState<Role | null>(null);
  const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);

  const rolesQuery = useQuery({
    queryKey: ['roles', page, pageSize],
    queryFn: () => fetchRoles(page, pageSize)
  });

  const permissionsQuery = useQuery({
    queryKey: ['permissions'],
    queryFn: fetchAllPermissions
  });

  const rolePermissionsQuery = useQuery({
    queryKey: ['role-permissions', assigningPermissionsRole?.id],
    queryFn: () => fetchRolePermissions(assigningPermissionsRole!.id),
    enabled: !!assigningPermissionsRole
  });

  useEffect(() => {
    if (rolePermissionsQuery.data) {
      setSelectedPermissions(rolePermissionsQuery.data.permissionIds);
    }
  }, [rolePermissionsQuery.data]);

  const columns = useMemo<ColumnDef<Role>[]>(
    () => [
      {
        accessorKey: 'name',
        header: 'Rol Adı',
        cell: ({ row }) => <span className="font-medium">{row.original.name}</span>
      },
      {
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setAssigningPermissionsRole(row.original)}
              aria-label="Yetki Ata"
              title="Yetki Ata"
            >
              <Shield className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="icon" onClick={() => setEditingRole(row.original)} aria-label="Düzenle">
              <Pencil className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="icon" onClick={() => setRoleToDelete(row.original)} aria-label="Sil">
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          </div>
        )
      }
    ],
    []
  );

  const table = useReactTable({
    data: rolesQuery.data?.items ?? [],
    columns,
    getCoreRowModel: getCoreRowModel()
  });

  const createMutation = useMutation({
    mutationFn: createRole,
    onSuccess: () => {
      toast.success('Rol başarıyla oluşturuldu');
      setIsCreateOpen(false);
      invalidateRoles();
      createForm.reset();
    },
    onError: () => toast.error('Rol oluşturulurken hata oluştu')
  });

  const updateMutation = useMutation({
    mutationFn: (values: RoleFormSchema) =>
      editingRole ? updateRole({ ...values, id: editingRole.id }) : Promise.reject(),
    onSuccess: () => {
      toast.success('Rol başarıyla güncellendi');
      setEditingRole(null);
      invalidateRoles();
    },
    onError: () => toast.error('Rol güncellenirken hata oluştu')
  });

  const deleteMutation = useMutation({
    mutationFn: (roleId: string) => deleteRole(roleId),
    onSuccess: () => {
      toast.success('Rol başarıyla silindi');
      setRoleToDelete(null);
      invalidateRoles();
    },
    onError: () => toast.error('Rol silinirken hata oluştu')
  });

  const assignPermissionsMutation = useMutation({
    mutationFn: (data: { roleId: string; permissionIds: string[] }) => assignPermissionsToRole(data),
    onSuccess: () => {
      toast.success('Yetkiler başarıyla atandı');
      setAssigningPermissionsRole(null);
      invalidateRoles();
    },
    onError: () => toast.error('Yetkiler atanırken hata oluştu')
  });

  const createForm = useForm<RoleFormSchema>({
    resolver: zodResolver(roleSchema),
    defaultValues: { name: '' }
  });

  const updateForm = useForm<RoleFormSchema>({
    resolver: zodResolver(roleSchema),
    defaultValues: { name: '' }
  });

  useEffect(() => {
    if (editingRole) {
      updateForm.reset({ name: editingRole.name });
    } else {
      updateForm.reset();
    }
  }, [editingRole, updateForm]);

  const handleAssignPermissions = () => {
    if (!assigningPermissionsRole) return;
    assignPermissionsMutation.mutate({
      roleId: assigningPermissionsRole.id,
      permissionIds: selectedPermissions
    });
  };

  const togglePermission = (permissionId: string) => {
    setSelectedPermissions((prev) =>
      prev.includes(permissionId) ? prev.filter((id) => id !== permissionId) : [...prev, permissionId]
    );
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Rol & Yetki Yönetimi</h1>
          <p className="text-muted-foreground mt-2">Rolleri ve yetkilerini yönetin</p>
        </div>
        <Button onClick={() => setIsCreateOpen(true)}>
          <PlusCircle className="w-4 h-4 mr-2" />
          Yeni Rol
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Roller</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="rounded-md border">
            <Table>
              <TableHeader>
                {table.getHeaderGroups().map((headerGroup) => (
                  <TableRow key={headerGroup.id}>
                    {headerGroup.headers.map((header) => (
                      <TableHead key={header.id}>
                        {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
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
                        <TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={columns.length} className="h-24 text-center">
                      {rolesQuery.isLoading ? 'Yükleniyor...' : 'Rol bulunamadı.'}
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>

          <div className="flex items-center justify-between mt-4">
            <div></div>
            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm" onClick={() => setPage((p) => Math.max(0, p - 1))} disabled={!rolesQuery.data?.hasPrevious}>
                Önceki
              </Button>
              <span className="text-sm">
                Sayfa {(rolesQuery.data?.index ?? 0) + 1} / {rolesQuery.data?.pages ?? 1}
              </span>
              <Button variant="outline" size="sm" onClick={() => setPage((p) => p + 1)} disabled={!rolesQuery.data?.hasNext}>
                Sonraki
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Create Dialog */}
      <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Yeni Rol Ekle</DialogTitle>
            <DialogDescription>Sisteme yeni rol ekleyin</DialogDescription>
          </DialogHeader>
          <form onSubmit={createForm.handleSubmit((values) => createMutation.mutate(values))} className="space-y-4">
            <div>
              <Label htmlFor="name">Rol Adı</Label>
              <Input id="name" {...createForm.register('name')} />
              {createForm.formState.errors.name && (
                <p className="text-sm text-destructive mt-1">{createForm.formState.errors.name.message}</p>
              )}
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setIsCreateOpen(false)}>İptal</Button>
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending ? 'Ekleniyor...' : 'Ekle'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* Update Dialog */}
      <Dialog open={!!editingRole} onOpenChange={() => setEditingRole(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Rol Düzenle</DialogTitle>
            <DialogDescription>Rol bilgilerini güncelleyin</DialogDescription>
          </DialogHeader>
          <form onSubmit={updateForm.handleSubmit((values) => updateMutation.mutate(values))} className="space-y-4">
            <div>
              <Label htmlFor="edit-name">Rol Adı</Label>
              <Input id="edit-name" {...updateForm.register('name')} />
              {updateForm.formState.errors.name && (
                <p className="text-sm text-destructive mt-1">{updateForm.formState.errors.name.message}</p>
              )}
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => setEditingRole(null)}>İptal</Button>
              <Button type="submit" disabled={updateMutation.isPending}>
                {updateMutation.isPending ? 'Güncelleniyor...' : 'Güncelle'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* Delete Dialog */}
      <Dialog open={!!roleToDelete} onOpenChange={() => setRoleToDelete(null)}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Rol Sil</DialogTitle>
            <DialogDescription>
              <strong>{roleToDelete?.name}</strong> rolünü silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setRoleToDelete(null)}>İptal</Button>
            <Button
              variant="destructive"
              onClick={() => roleToDelete && deleteMutation.mutate(roleToDelete.id)}
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? 'Siliniyor...' : 'Sil'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Assign Permissions Dialog */}
      <Dialog open={!!assigningPermissionsRole} onOpenChange={() => setAssigningPermissionsRole(null)}>
        <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Yetki Ata</DialogTitle>
            <DialogDescription>
              <strong>{assigningPermissionsRole?.name}</strong> rolüne yetkiler atayın
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-6">
            {permissionsQuery.isLoading && <p className="text-sm text-muted-foreground">Yetkiler yükleniyor...</p>}
            {permissionsQuery.data?.modules.map((module) => (
              <div key={module.moduleName} className="space-y-3">
                <h3 className="font-semibold text-sm border-b pb-2">{module.moduleName}</h3>
                <div className="grid grid-cols-2 gap-3">
                  {module.permissions.map((permission) => (
                    <div key={permission.id} className="flex items-start space-x-2">
                      <Checkbox
                        id={`permission-${permission.id}`}
                        checked={selectedPermissions.includes(permission.id)}
                        onCheckedChange={() => togglePermission(permission.id)}
                      />
                      <div className="grid gap-1.5 leading-none">
                        <label
                          htmlFor={`permission-${permission.id}`}
                          className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                        >
                          {permission.type}
                        </label>
                        {permission.description && (
                          <p className="text-xs text-muted-foreground">{permission.description}</p>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setAssigningPermissionsRole(null)}>İptal</Button>
            <Button onClick={handleAssignPermissions} disabled={assignPermissionsMutation.isPending}>
              {assignPermissionsMutation.isPending ? 'Atanıyor...' : 'Kaydet'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
