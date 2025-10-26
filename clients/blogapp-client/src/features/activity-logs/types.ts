export interface ActivityLog {
  id: string;
  activityType: string;
  entityType: string;
  entityId: string | null;
  title: string;
  details: string | null;
  userId: string | null;
  userName: string | null;
  timestamp: string;
}

export interface ActivityLogFilters {
  activityType?: string;
  entityType?: string;
  userId?: string;
  startDate?: string;
  endDate?: string;
}
