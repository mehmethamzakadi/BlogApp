import { useQueryClient } from '@tanstack/react-query';

/**
 * Cache invalidation için merkezi hook
 * Her mutation sonrasında ilgili tüm cache'leri invalidate eder
 */
export function useInvalidateQueries() {
  const queryClient = useQueryClient();

  /**
   * Kullanıcı işlemleri sonrası invalidation
   */
  const invalidateUsers = () => {
    queryClient.invalidateQueries({ queryKey: ['users'] });
    queryClient.invalidateQueries({ queryKey: ['user-roles'] });
    queryClient.invalidateQueries({ queryKey: ['activity-logs'] });
    queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
  };

  /**
   * Rol işlemleri sonrası invalidation
   */
  const invalidateRoles = () => {
    queryClient.invalidateQueries({ queryKey: ['roles'] });
    queryClient.invalidateQueries({ queryKey: ['role-permissions'] });
    queryClient.invalidateQueries({ queryKey: ['activity-logs'] });
    queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
  };

  /**
   * Post işlemleri sonrası invalidation
   */
  const invalidatePosts = () => {
    queryClient.invalidateQueries({ queryKey: ['posts'] });
    queryClient.invalidateQueries({ queryKey: ['posts', 'published'] });
    queryClient.invalidateQueries({ queryKey: ['dashboard-statistics'] });
    queryClient.invalidateQueries({ queryKey: ['activity-logs'] });
    queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
  };

  /**
   * Kategori işlemleri sonrası invalidation
   */
  const invalidateCategories = () => {
    queryClient.invalidateQueries({ queryKey: ['categories'] });
    queryClient.invalidateQueries({ queryKey: ['dashboard-statistics'] });
    queryClient.invalidateQueries({ queryKey: ['activity-logs'] });
    queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
  };

  /**
   * Bookshelf kayıtları sonrasında invalidation
   */
  const invalidateBookshelf = () => {
    queryClient.invalidateQueries({ queryKey: ['bookshelf-items'] });
    queryClient.invalidateQueries({ queryKey: ['activity-logs'] });
    queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
  };

  /**
   * Yorum işlemleri sonrası invalidation
   */
  const invalidateComments = () => {
    queryClient.invalidateQueries({ queryKey: ['comments'] });
    queryClient.invalidateQueries({ queryKey: ['activity-logs'] });
    queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
  };

  /**
   * Activity log'ları invalidate et
   */
  const invalidateActivityLogs = () => {
    queryClient.invalidateQueries({ queryKey: ['activity-logs'] });
    queryClient.invalidateQueries({ queryKey: ['recent-activities'] });
  };

  /**
   * Tüm cache'leri temizle (logout vs. için)
   */
  const invalidateAll = () => {
    queryClient.clear();
  };

  return {
    invalidateUsers,
    invalidateRoles,
    invalidatePosts,
    invalidateCategories,
    invalidateBookshelf,
    invalidateComments,
    invalidateActivityLogs,
    invalidateAll
  };
}
