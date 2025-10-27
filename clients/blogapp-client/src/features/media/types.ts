export interface ImageUploadOptions {
  scope?: string;
  resizeMode?: 'fit' | 'crop';
  maxWidth?: number;
  maxHeight?: number;
  title?: string;
}

export interface UploadedImage {
  imageId: string;
  fileName: string;
  contentType: string;
  size: number;
  relativePath: string;
  url: string;
  width: number;
  height: number;
}
