import { Navigate, createBrowserRouter } from 'react-router-dom';
import { PublicLayout } from '../components/layout/public-layout';
import { HomePage } from '../pages/public/home-page';
import { LoginPage } from '../pages/public/login-page';
import { RegisterPage } from '../pages/public/register-page';
import { PostDetailPage } from '../pages/public/post-detail-page';
import { ProtectedRoute } from './protected-route';
import { AdminLayout } from '../components/layout/admin-layout';
import { DashboardPage } from '../pages/admin/dashboard-page';
import { CategoriesPage } from '../pages/admin/categories-page';
import { PostsPage } from '../pages/admin/posts-page';
import { CreatePostPage } from '../pages/admin/create-post-page';
import { UsersPage } from '../pages/admin/users-page';
import { RolesPage } from '../pages/admin/roles-page';
import { ActivityLogsPage } from '../pages/admin/activity-logs-page';
import ForbiddenPage from '../pages/ForbiddenPage';
import { Permissions } from '../lib/permissions';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <PublicLayout />,
    children: [
      {
        index: true,
        element: <HomePage />
      },
      {
        path: 'login',
        element: <LoginPage />
      },
      {
        path: 'register',
        element: <RegisterPage />
      },
      {
        path: 'posts/:postId',
        element: <PostDetailPage />
      }
    ]
  },
  {
    path: '/admin',
    element: (
      <ProtectedRoute>
        <AdminLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <Navigate to="dashboard" replace />
      },
      {
        path: 'dashboard',
        element: (
          <ProtectedRoute requiredPermission={Permissions.DashboardView}>
            <DashboardPage />
          </ProtectedRoute>
        )
      },
      {
        path: 'categories',
        element: (
          <ProtectedRoute requiredPermission={Permissions.CategoriesViewAll}>
            <CategoriesPage />
          </ProtectedRoute>
        )
      },
      {
        path: 'posts',
        element: (
          <ProtectedRoute requiredPermission={Permissions.PostsViewAll}>
            <PostsPage />
          </ProtectedRoute>
        )
      },
      {
        path: 'posts/new',
        element: (
          <ProtectedRoute requiredPermission={Permissions.PostsCreate}>
            <CreatePostPage />
          </ProtectedRoute>
        )
      },
      {
        path: 'posts/:postId/edit',
        element: (
          <ProtectedRoute requiredPermission={Permissions.PostsUpdate}>
            <CreatePostPage />
          </ProtectedRoute>
        )
      },
      {
        path: 'users',
        element: (
          <ProtectedRoute requiredPermission={Permissions.UsersViewAll}>
            <UsersPage />
          </ProtectedRoute>
        )
      },
      {
        path: 'roles',
        element: (
          <ProtectedRoute requiredPermission={Permissions.RolesViewAll}>
            <RolesPage />
          </ProtectedRoute>
        )
      },
      {
        path: 'activity-logs',
        element: (
          <ProtectedRoute requiredPermission={Permissions.DashboardView}>
            <ActivityLogsPage />
          </ProtectedRoute>
        )
      }
    ]
  },
  {
    path: '/forbidden',
    element: <ForbiddenPage />
  },
  {
    path: '*',
    element: <Navigate to="/" replace />
  }
]);
