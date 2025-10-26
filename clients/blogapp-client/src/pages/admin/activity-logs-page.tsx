import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import { DataGridRequest, FilterDescriptor } from '../../types/api';
import { getActivityLogs } from '../../features/activity-logs/api';
import { ActivityLog } from '../../features/activity-logs/types';
import { 
  Activity, 
  User, 
  FileText, 
  Folder, 
  Shield,
  Clock,
  Filter,
  Search,
  RefreshCw
} from 'lucide-react';
import { Input } from '../../components/ui/input';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
import { formatDistanceToNow } from 'date-fns';
import { tr } from 'date-fns/locale';

const ACTIVITY_TYPES = [
  { value: '', label: 'Tümü' },
  { value: 'user_created', label: 'Kullanıcı Oluşturma' },
  { value: 'user_updated', label: 'Kullanıcı Güncelleme' },
  { value: 'user_deleted', label: 'Kullanıcı Silme' },
  { value: 'role_created', label: 'Rol Oluşturma' },
  { value: 'role_updated', label: 'Rol Güncelleme' },
  { value: 'role_deleted', label: 'Rol Silme' },
  { value: 'post_created', label: 'Post Oluşturma' },
  { value: 'post_updated', label: 'Post Güncelleme' },
  { value: 'post_deleted', label: 'Post Silme' },
  { value: 'category_created', label: 'Kategori Oluşturma' },
  { value: 'category_updated', label: 'Kategori Güncelleme' },
  { value: 'category_deleted', label: 'Kategori Silme' },
];

const ENTITY_TYPES = [
  { value: '', label: 'Tümü' },
  { value: 'User', label: 'Kullanıcı' },
  { value: 'Role', label: 'Rol' },
  { value: 'Post', label: 'Post' },
  { value: 'Category', label: 'Kategori' },
];

function getActivityIcon(entityType: string) {
  switch (entityType.toLowerCase()) {
    case 'user':
      return <User className="w-4 h-4" />;
    case 'role':
      return <Shield className="w-4 h-4" />;
    case 'post':
      return <FileText className="w-4 h-4" />;
    case 'category':
      return <Folder className="w-4 h-4" />;
    default:
      return <Activity className="w-4 h-4" />;
  }
}

function getActivityColor(activityType: string) {
  if (activityType.includes('created')) return 'bg-green-500/10 text-green-700 dark:text-green-400';
  if (activityType.includes('updated')) return 'bg-blue-500/10 text-blue-700 dark:text-blue-400';
  if (activityType.includes('deleted')) return 'bg-red-500/10 text-red-700 dark:text-red-400';
  return 'bg-gray-500/10 text-gray-700 dark:text-gray-400';
}

export function ActivityLogsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [searchTerm, setSearchTerm] = useState('');
  const [activityTypeFilter, setActivityTypeFilter] = useState('');
  const [entityTypeFilter, setEntityTypeFilter] = useState('');

  const buildRequest = (): DataGridRequest => {
    const filters: FilterDescriptor[] = [];

    if (searchTerm) {
      filters.push({
        field: 'Title',
        operator: 'contains',
        value: searchTerm
      });
    }

    if (activityTypeFilter) {
      filters.push({
        field: 'ActivityType',
        operator: 'eq',
        value: activityTypeFilter
      });
    }

    if (entityTypeFilter) {
      filters.push({
        field: 'EntityType',
        operator: 'eq',
        value: entityTypeFilter
      });
    }

    const filterDescriptor = filters.length === 0
      ? undefined
      : filters.length === 1
        ? filters[0]
        : {
            field: '',
            operator: '',
            logic: 'and',
            filters
          } satisfies FilterDescriptor;

    return {
      paginatedRequest: {
        pageIndex: page - 1,
        pageSize: pageSize
      },
      dynamicQuery: {
        sort: [{ field: 'Timestamp', dir: 'desc' }],
        ...(filterDescriptor ? { filter: filterDescriptor } : {})
      }
    };
  };

  const { data, isLoading, refetch, isFetching } = useQuery({
    queryKey: ['activity-logs', page, pageSize, searchTerm, activityTypeFilter, entityTypeFilter],
    queryFn: () => getActivityLogs(buildRequest()),
  });

  const handleSearch = () => {
    setPage(1);
    refetch();
  };

  const handleReset = () => {
    setSearchTerm('');
    setActivityTypeFilter('');
    setEntityTypeFilter('');
    setPage(1);
  };

  const totalPages = data ? data.pages : 0;

  return (
    <div className="container mx-auto py-6 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight flex items-center gap-2">
            <Activity className="w-8 h-8" />
            Sistem Aktiviteleri
          </h1>
          <p className="text-muted-foreground mt-1">
            Sistemde gerçekleşen tüm aktiviteleri görüntüleyin ve filtreleyin
          </p>
        </div>
        <Button
          onClick={() => refetch()}
          disabled={isFetching}
          variant="outline"
          size="sm"
        >
          <RefreshCw className={`w-4 h-4 mr-2 ${isFetching ? 'animate-spin' : ''}`} />
          Yenile
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Filter className="w-5 h-5" />
            Filtreler
          </CardTitle>
          <CardDescription>
            Aktiviteleri filtrelemek için aşağıdaki seçenekleri kullanın
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="md:col-span-2">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                <Input
                  placeholder="Aktivite ara..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                  className="pl-9"
                />
              </div>
            </div>

            <select
              value={activityTypeFilter}
              onChange={(e) => {
                setActivityTypeFilter(e.target.value);
                setPage(1);
              }}
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
            >
              {ACTIVITY_TYPES.map((type) => (
                <option key={type.value} value={type.value}>
                  {type.label}
                </option>
              ))}
            </select>

            <select
              value={entityTypeFilter}
              onChange={(e) => {
                setEntityTypeFilter(e.target.value);
                setPage(1);
              }}
              className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
            >
              {ENTITY_TYPES.map((type) => (
                <option key={type.value} value={type.value}>
                  {type.label}
                </option>
              ))}
            </select>
          </div>

          <div className="flex gap-2 mt-4">
            <Button onClick={handleSearch} disabled={isFetching}>
              <Search className="w-4 h-4 mr-2" />
              Ara
            </Button>
            <Button onClick={handleReset} variant="outline" disabled={isFetching}>
              Temizle
            </Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Aktivite Listesi</CardTitle>
          <CardDescription>
            {data && `Toplam ${data.count} aktivite bulundu`}
          </CardDescription>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="flex items-center justify-center py-12">
              <RefreshCw className="w-8 h-8 animate-spin text-muted-foreground" />
            </div>
          ) : data && data.items.length > 0 ? (
            <div className="space-y-3">
              {data.items.map((log: ActivityLog) => (
                <div
                  key={log.id}
                  className="flex items-start gap-4 p-4 rounded-lg border bg-card hover:bg-accent/50 transition-colors"
                >
                  <div className={`p-2 rounded-full ${getActivityColor(log.activityType)}`}>
                    {getActivityIcon(log.entityType)}
                  </div>

                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-2">
                      <div className="flex-1">
                        <p className="font-medium text-sm">{log.title}</p>
                        {log.details && (
                          <p className="text-sm text-muted-foreground mt-1 line-clamp-2">
                            {log.details}
                          </p>
                        )}
                        <div className="flex items-center gap-3 mt-2 text-xs text-muted-foreground">
                          {log.userName && (
                            <span className="flex items-center gap-1">
                              <User className="w-3 h-3" />
                              {log.userName}
                            </span>
                          )}
                          <span className="flex items-center gap-1">
                            <Clock className="w-3 h-3" />
                            {formatDistanceToNow(new Date(log.timestamp), {
                              addSuffix: true,
                              locale: tr
                            })}
                          </span>
                        </div>
                      </div>

                      <div className="flex flex-col items-end gap-2">
                        <Badge variant="outline" className="text-xs">
                          {log.entityType}
                        </Badge>
                        {log.entityId && (
                          <span className="text-xs text-muted-foreground">
                            ID: {log.entityId}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-12 text-muted-foreground">
              <Activity className="w-12 h-12 mx-auto mb-4 opacity-50" />
              <p>Henüz aktivite kaydı bulunmamaktadır.</p>
            </div>
          )}

          {/* Pagination */}
          {data && data.items.length > 0 && (
            <div className="flex items-center justify-between mt-6 pt-4 border-t">
              <div className="text-sm text-muted-foreground">
                Sayfa {page} / {totalPages} ({data.count} kayıt)
              </div>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1 || isFetching}
                >
                  Önceki
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages || isFetching}
                >
                  Sonraki
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
