import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { RichTextEditor } from '../../components/editor/rich-text-editor';

import { Button } from '../../components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { createPost } from '../../features/posts/api';
import { postSchema, PostFormSchema } from '../../features/posts/schema';
import { PostFormValues } from '../../features/posts/types';
import { getAllCategories } from '../../features/categories/api';
import { Category } from '../../features/categories/types';
import { useUnsavedChangesWarning } from '../../hooks/use-unsaved-changes-warning';

const textareaBaseClasses =
  'flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50';

const confirmMessage =
  'Kaydedilmemiş değişiklikler var. Taslak olarak kaydetmeden çıkmak istediğinize emin misiniz?';

export function CreatePostPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [hasSaved, setHasSaved] = useState(false);

  const categoriesQuery = useQuery<Category[]>({
    queryKey: ['categories-options'],
    queryFn: getAllCategories
  });

  const defaultCategoryId = useMemo(() => categoriesQuery.data?.[0]?.id ?? 0, [categoriesQuery.data]);

  const formMethods = useForm<PostFormSchema>({
    resolver: zodResolver(postSchema),
    defaultValues: {
      title: '',
      summary: '',
      body: '',
      thumbnail: '',
      isPublished: false,
      categoryId: defaultCategoryId
    }
  });

  const {
    handleSubmit,
    register,
    control,
    watch,
    formState,
    setValue,
    getValues,
    reset
  } = formMethods;

  const isPublished = watch('isPublished');

  useEffect(() => {
    if (!categoriesQuery.data?.length) {
      return;
    }

    if (getValues('categoryId') === 0) {
      setValue('categoryId', categoriesQuery.data[0].id, { shouldDirty: false });
    }
  }, [categoriesQuery.data, getValues, setValue]);

  const createMutation = useMutation({
    mutationFn: (values: PostFormValues) => createPost(values),
    onSuccess: async (result) => {
      if (!result.success) {
        toast.error(result.message || 'Gönderi oluşturulamadı');
        return;
      }

      setHasSaved(true);
      toast.success(result.message || 'Gönderi oluşturuldu');
      await queryClient.invalidateQueries({ queryKey: ['posts'] });
      await queryClient.invalidateQueries({ queryKey: ['posts', 'published'] });
      reset({
        title: '',
        summary: '',
        body: '',
        thumbnail: '',
        isPublished: false,
        categoryId: categoriesQuery.data?.[0]?.id ?? 0
      });
      navigate('/admin/posts');
    },
    onError: () => toast.error('Gönderi oluşturulurken bir hata oluştu')
  });

  const shouldBlockNavigation = formState.isDirty && !hasSaved && !createMutation.isPending;
  useUnsavedChangesWarning(shouldBlockNavigation, confirmMessage);

  const onSubmit = handleSubmit(async (values) => {
    await createMutation.mutateAsync(values);
  });

  const handleBack = () => {
    if (shouldBlockNavigation && !window.confirm(confirmMessage)) {
      return;
    }

    navigate('/admin/posts');
  };

  const submitLabel = createMutation.isPending
    ? 'Kaydediliyor...'
    : isPublished
      ? 'Yayınla'
      : 'Taslak Olarak Kaydet';

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 rounded-xl border bg-card p-6 shadow-sm sm:flex-row sm:items-center sm:justify-between">
        <div className="space-y-2">
          <h1 className="text-2xl font-semibold tracking-tight">Yeni Gönderi Oluştur</h1>
          <p className="text-sm text-muted-foreground">
            Gönderinizin detaylarını girin, ister hemen yayınlayın ister taslak olarak kaydedin.
          </p>
        </div>
        <Button variant="outline" onClick={handleBack} className="self-start sm:self-auto">
          Geri
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Gönderi Bilgileri</CardTitle>
          <CardDescription>Başlık, özet ve içerik alanlarını doldurarak gönderinizi hazırlayın.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={onSubmit} className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="create-post-title">Başlık</Label>
              <Input
                id="create-post-title"
                placeholder="Gönderi başlığı"
                {...register('title')}
              />
              {formState.errors.title && (
                <p className="text-sm text-destructive">{formState.errors.title.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="create-post-summary">Özet</Label>
              <textarea
                id="create-post-summary"
                placeholder="Gönderinin kısa özetini yazın"
                className={textareaBaseClasses + ' min-h-[120px]'}
                {...register('summary')}
              />
              {formState.errors.summary && (
                <p className="text-sm text-destructive">{formState.errors.summary.message}</p>
              )}
            </div>

            <Controller
              name="body"
              control={control}
              render={({ field }) => (
                <div className="space-y-2">
                  <Label>İçerik</Label>
                  <RichTextEditor
                    value={field.value}
                    onChange={field.onChange}
                    placeholder="Gönderi içeriğini yazmaya başlayın"
                  />
                  {formState.errors.body && (
                    <p className="text-sm text-destructive">{formState.errors.body.message}</p>
                  )}
                </div>
              )}
            />

            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="create-post-category">Kategori</Label>
                <select
                  id="create-post-category"
                  className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                  {...register('categoryId', { valueAsNumber: true })}
                >
                  <option value={0} disabled>
                    Kategori seçin
                  </option>
                  {categoriesQuery.data?.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))}
                </select>
                {formState.errors.categoryId && (
                  <p className="text-sm text-destructive">{formState.errors.categoryId.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="create-post-thumbnail">Küçük Görsel URL</Label>
                <Input
                  id="create-post-thumbnail"
                  placeholder="https://..."
                  {...register('thumbnail')}
                />
                {formState.errors.thumbnail && (
                  <p className="text-sm text-destructive">{formState.errors.thumbnail.message}</p>
                )}
              </div>
            </div>

            <div className="flex flex-col gap-4 border-t pt-4 sm:flex-row sm:items-center sm:justify-between">
              <label className="inline-flex items-center gap-2 text-sm font-medium">
                <input
                  type="checkbox"
                  className="h-4 w-4 rounded border border-input"
                  {...register('isPublished')}
                />
                Gönderiyi Yayınla
              </label>
              <div className="flex gap-2">
                <Button type="button" variant="outline" onClick={handleBack}>
                  Vazgeç
                </Button>
                <Button type="submit" disabled={createMutation.isPending}>
                  {submitLabel}
                </Button>
              </div>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
