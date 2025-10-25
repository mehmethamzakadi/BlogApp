export interface DashboardStatCard {
  title: string;
  value: number | string;
  description: string;
  icon: React.ComponentType<{ className?: string }>;
  trend?: {
    value: number;
    isPositive: boolean;
  };
  color?: 'blue' | 'green' | 'yellow' | 'purple' | 'red';
}

export interface RecentActivity {
  id: number;
  type: 'post_created' | 'post_updated' | 'post_deleted' | 'category_created';
  title: string;
  description: string;
  timestamp: Date;
  user?: string;
}

export interface ChartDataPoint {
  name: string;
  value: number;
  label?: string;
}
