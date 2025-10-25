import { motion } from 'framer-motion';
import { FileText, FolderKanban, Plus, Trash2 } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
import { Separator } from '../ui/separator';
import { formatDistanceToNow } from 'date-fns';
import { tr } from 'date-fns/locale';

export interface Activity {
  id: number;
  type: 'post_created' | 'post_updated' | 'post_deleted' | 'category_created';
  title: string;
  timestamp: Date | string;
}

interface ActivityFeedProps {
  activities: Activity[];
  delay?: number;
}

const activityIcons = {
  post_created: Plus,
  post_updated: FileText,
  post_deleted: Trash2,
  category_created: FolderKanban
};

const activityColors = {
  post_created: 'text-green-600 dark:text-green-400 bg-green-500/10',
  post_updated: 'text-blue-600 dark:text-blue-400 bg-blue-500/10',
  post_deleted: 'text-red-600 dark:text-red-400 bg-red-500/10',
  category_created: 'text-purple-600 dark:text-purple-400 bg-purple-500/10'
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
              const Icon = activityIcons[activity.type];
              const colorClass = activityColors[activity.type];
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
                      <p className="text-xs text-muted-foreground">
                        {formatDistanceToNow(timestamp, { addSuffix: true, locale: tr })}
                      </p>
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
