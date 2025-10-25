import { z } from 'zod';

export const postSchema = z.object({
  title: z.string().min(1, 'Başlık boş olamaz').max(100, 'Başlık en fazla 100 karakter olabilir'),
  summary: z.string().min(1, 'Özet boş olamaz').max(400, 'Özet en fazla 400 karakter olabilir'),
  body: z.string().min(1, 'İçerik boş olamaz'),
  thumbnail: z
    .string()
    .trim()
    .refine(
      (value) => value.length === 0 || /^https?:\/\/.+/i.test(value),
      'Geçerli bir URL girin'
    ),
  isPublished: z.boolean(),
  categoryId: z.number().int().positive('Kategori seçilmelidir')
});

export type PostFormSchema = z.infer<typeof postSchema>;
