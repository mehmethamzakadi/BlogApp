import { motion } from 'framer-motion';
import { FileText, FolderKanban, Plus, Trash2, Edit, User } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
import { Separator } from '../ui/separator';
import { formatDistanceToNow } from 'date-fns';
import { tr } from 'date-fns/locale';

export interface Activity {
  id: string;
  activityType: string;
  title: string;
  timestamp: Date | string;
  userName?: string;
}

interface ActivityFeedProps {
  activities: Activity[];
  delay?: number;
}

const activityIcons: Record<string, any> = {
  post_created: Plus,
  post_updated: Edit,
  post_deleted: Trash2,
  category_created: FolderKanban,
  category_updated: Edit,
  category_deleted: Trash2,
  user_created: User,
  user_updated: Edit,
  user_deleted: Trash2
};

const activityColors: Record<string, string> = {
  post_created: 'text-green-600 dark:text-green-400 bg-green-500/10',
  post_updated: 'text-blue-600 dark:text-blue-400 bg-blue-500/10',
  post_deleted: 'text-red-600 dark:text-red-400 bg-red-500/10',
  category_created: 'text-purple-600 dark:text-purple-400 bg-purple-500/10',
  category_updated: 'text-blue-600 dark:text-blue-400 bg-blue-500/10',
  category_deleted: 'text-red-600 dark:text-red-400 bg-red-500/10',
  user_created: 'text-green-600 dark:text-green-400 bg-green-500/10',
  user_updated: 'text-blue-600 dark:text-blue-400 bg-blue-500/10',
  user_deleted: 'text-red-600 dark:text-red-400 bg-red-500/10'
};

export function ActivityFeed({ activities, delay = 0 }: ActivityFeedProps) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay, duration: 0.3 }}
    >
      <Card>
        <CardHeader>
          <CardTitle>Son Aktiviteler</CardTitle>
          <CardDescription>Sistemdeki son değişiklikleri görüntüleyin</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {activities.length === 0 ? (
            <p className="text-center text-sm text-muted-foreground">Henüz aktivite bulunmuyor</p>
          ) : (
            activities.map((activity, index) => {
              const Icon = activityIcons[activity.activityType] || FileText;
              const colorClass = activityColors[activity.activityType] || 'text-gray-600 dark:text-gray-400 bg-gray-500/10';
              const timestamp =
                typeof activity.timestamp === 'string' ? new Date(activity.timestamp) : activity.timestamp;

              return (
                <div key={activity.id}>
                  <div className="flex items-start gap-4">
                    <div className={`rounded-lg p-2 ${colorClass}`}>
                      <Icon className="h-4 w-4" />
                    </div>
                    <div className="flex-1 space-y-1">
                      <p className="text-sm font-medium leading-none">{activity.title}</p>
                      <div className="flex items-center gap-2 text-xs text-muted-foreground">
                        <span>{formatDistanceToNow(timestamp, { addSuffix: true, locale: tr })}</span>
                        {activity.userName && (
                          <>
                            <span>•</span>
                            <span>{activity.userName}</span>
                          </>
                        )}
                      </div>
                    </div>
                  </div>
                  {index < activities.length - 1 && <Separator className="mt-4" />}
                </div>
              );
            })
          )}
        </CardContent>
      </Card>
    </motion.div>
  );
}
