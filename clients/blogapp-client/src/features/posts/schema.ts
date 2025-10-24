import { z } from 'zod';

export const postSchema = z.object({
  title: z.string().min(3, 'Başlık en az 3 karakter olmalıdır'),
  summary: z.string().min(10, 'Özet en az 10 karakter olmalıdır'),
  body: z.string().min(20, 'İçerik en az 20 karakter olmalıdır'),
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
