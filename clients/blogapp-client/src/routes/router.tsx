import { Navigate, createBrowserRouter } from 'react-router-dom';
import { PublicLayout } from '../components/layout/public-layout';
import { HomePage } from '../pages/public/home-page';
import { LoginPage } from '../pages/public/login-page';
import { PostDetailPage } from '../pages/public/post-detail-page';
import { ProtectedRoute } from './protected-route';
import { AdminLayout } from '../components/layout/admin-layout';
import { DashboardPage } from '../pages/admin/dashboard-page';
import { CategoriesPage } from '../pages/admin/categories-page';
import { PostsPage } from '../pages/admin/posts-page';
import { CreatePostPage } from '../pages/admin/create-post-page';

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
        element: <DashboardPage />
      },
      {
        path: 'categories',
        element: <CategoriesPage />
      },
      {
        path: 'posts',
        element: <PostsPage />
      },
      {
        path: 'posts/new',
        element: <CreatePostPage />
      },
      {
        path: 'posts/:postId/edit',
        element: <CreatePostPage />
      }
    ]
  },
  {
    path: '*',
    element: <Navigate to="/" replace />
  }
]);
