export interface ActivityLog {
  id: number;
  activityType: string;
  entityType: string;
  entityId: number | null;
  title: string;
  details: string | null;
  userId: number | null;
  userName: string | null;
  timestamp: string;
}

export interface ActivityLogFilters {
  activityType?: string;
  entityType?: string;
  userId?: number;
  startDate?: string;
  endDate?: string;
}
