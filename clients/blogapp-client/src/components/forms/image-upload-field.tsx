import { ChangeEvent, useRef } from 'react';
import { useMutation } from '@tanstack/react-query';
import toast from 'react-hot-toast';

import { uploadImage } from '../../features/media/api';
import { ImageUploadOptions } from '../../features/media/types';
import { Permissions } from '../../lib/permissions';
import { resolveApiAssetUrl } from '../../lib/utils';
import { Button } from '../ui/button';
import { Label } from '../ui/label';
import { usePermission } from '../../hooks/use-permission';

interface ImageUploadFieldProps extends ImageUploadOptions {
  value: string;
  onChange: (value: string) => void;
  onRemove: () => void;
  isDisabled?: boolean;
  label?: string;
  description?: string;
}

export function ImageUploadField({
  value,
  onChange,
  onRemove,
  isDisabled,
  label,
  description,
  scope,
  resizeMode,
  maxWidth,
  maxHeight,
  title
}: ImageUploadFieldProps) {
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const { hasPermission } = usePermission();
  const canUpload = hasPermission(Permissions.MediaUpload);

  const uploadMutation = useMutation({
    mutationFn: async (file: File) => uploadImage(file, { scope, resizeMode, maxWidth, maxHeight, title }),
    onSuccess: (result) => {
      if (result.success && result.data) {
        onChange(result.data.url);
        toast.success('Görsel başarıyla yüklendi.');
      } else {
        toast.error(result.message || 'Görsel yüklenirken bir sorun oluştu.');
      }
    },
    onError: (error: any) => {
      const message = typeof error?.message === 'string' ? error.message : 'Görsel yüklenirken bir hata oluştu.';
      toast.error(message);
    }
  });

  const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    event.target.value = '';

    if (!file) {
      return;
    }

    if (!canUpload) {
      toast.error('Görsel yükleme yetkiniz bulunmuyor.');
      return;
    }

    uploadMutation.mutate(file);
  };

  const handleSelectClick = () => {
    if (!canUpload || isDisabled) {
      return;
    }

    fileInputRef.current?.click();
  };

  const previewUrl = resolveApiAssetUrl(value);

  return (
    <div className="space-y-2">
      {label && <Label>{label}</Label>}
      {description && <p className="text-sm text-muted-foreground">{description}</p>}

      <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
        <div className="flex h-28 w-28 items-center justify-center overflow-hidden rounded-md border bg-muted">
          {previewUrl ? (
            <img src={previewUrl} alt="Seçili görsel" className="h-full w-full object-cover" />
          ) : (
            <span className="text-xs text-muted-foreground">Görsel Yok</span>
          )}
        </div>

        <div className="flex flex-1 flex-col gap-2">
          <div className="flex flex-wrap items-center gap-2">
            <Button
              type="button"
              variant="outline"
              onClick={handleSelectClick}
              disabled={isDisabled || uploadMutation.isPending || !canUpload}
            >
              {uploadMutation.isPending ? 'Yükleniyor...' : 'Görsel Seç'}
            </Button>
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*"
              className="hidden"
              onChange={handleFileChange}
            />

            {value && (
              <Button
                type="button"
                variant="ghost"
                onClick={onRemove}
                disabled={isDisabled || uploadMutation.isPending || !canUpload}
              >
                Görseli Kaldır
              </Button>
            )}
          </div>
          {!canUpload && (
            <p className="text-xs text-muted-foreground">
              Görsel yüklemek için gerekli izne sahip değilsiniz.
            </p>
          )}
        </div>
      </div>
    </div>
  );
}
