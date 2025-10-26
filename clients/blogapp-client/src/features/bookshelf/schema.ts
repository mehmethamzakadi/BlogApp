import { z } from 'zod';

const optionalText = (max: number, message: string) =>
  z
    .string()
    .trim()
    .max(max, message)
    .or(z.literal(''))
    .transform((value) => value.trim());

export const bookshelfItemSchema = z
  .object({
    title: z
      .string()
      .trim()
      .min(1, 'Kitap adı boş olamaz')
      .max(200, 'Kitap adı en fazla 200 karakter olabilir'),
    author: optionalText(150, 'Yazar bilgisi en fazla 150 karakter olabilir'),
    publisher: optionalText(150, 'Yayınevi bilgisi en fazla 150 karakter olabilir'),
    pageCount: z
      .string()
      .trim()
      .or(z.literal(''))
      .refine((value) => {
        if (value.length === 0) {
          return true;
        }

        if (!/^\d+$/.test(value)) {
          return false;
        }

        const numeric = Number(value);
        return numeric > 0 && numeric <= 20000;
      }, 'Sayfa sayısı 1 ile 20000 arasında olmalıdır'),
    isRead: z.boolean(),
    notes: optionalText(2000, 'Not alanı en fazla 2000 karakter olabilir'),
    readDate: z
      .string()
      .trim()
      .or(z.literal(''))
  })
  .superRefine((data, ctx) => {
    if (data.isRead) {
      if (!data.readDate) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          path: ['readDate'],
          message: 'Okunma tarihi, kitap okundu olarak işaretlendiğinde zorunludur'
        });
        return;
      }

      const parsed = new Date(data.readDate);
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      if (Number.isNaN(parsed.getTime())) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          path: ['readDate'],
          message: 'Geçerli bir tarih seçin'
        });
        return;
      }

      const normalized = new Date(parsed);
      normalized.setHours(0, 0, 0, 0);
      if (normalized > today) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          path: ['readDate'],
          message: 'Okunma tarihi gelecekte olamaz'
        });
      }
    } else if (data.readDate) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        path: ['readDate'],
        message: 'Okunma tarihi yalnızca kitap okundu olarak işaretlendiğinde girilmelidir'
      });
    }
  });

export type BookshelfItemFormSchema = z.infer<typeof bookshelfItemSchema>;
