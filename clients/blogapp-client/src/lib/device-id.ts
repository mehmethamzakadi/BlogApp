/**
 * DeviceId - Cihaz bazlı oturum yönetimi için benzersiz tanımlayıcı
 * 
 * Her cihaz/tarayıcı kombinasyonu için benzersiz bir UUID oluşturur ve
 * localStorage'da saklar. Bu sayede sunucu tarafında kullanıcının hangi
 * cihazlardan oturum açtığını takip edebilir.
 */

const DEVICE_ID_KEY = 'device_id';

/**
 * Mevcut cihaz ID'sini getirir, yoksa yeni bir tane oluşturur
 */
export const getOrCreateDeviceId = (): string => {
  try {
    let deviceId = localStorage.getItem(DEVICE_ID_KEY);
    
    if (!deviceId) {
      // crypto.randomUUID() modern tarayıcılarda desteklenir
      if (crypto && crypto.randomUUID) {
        deviceId = crypto.randomUUID();
      } else {
        // Fallback için basit UUID v4 oluştur
        deviceId = generateUUIDv4();
      }
      
      localStorage.setItem(DEVICE_ID_KEY, deviceId);
    }
    
    return deviceId;
  } catch (error) {
    // localStorage erişilemiyorsa veya hata olursa geçici ID oluştur
    console.warn('DeviceId localStorage hatası:', error);
    return generateUUIDv4();
  }
};

/**
 * Mevcut cihaz ID'sini getirir (varsa)
 */
export const getDeviceId = (): string | null => {
  try {
    return localStorage.getItem(DEVICE_ID_KEY);
  } catch {
    return null;
  }
};

/**
 * Cihaz ID'sini siler (logout gibi durumlarda kullanılabilir)
 */
export const clearDeviceId = (): void => {
  try {
    localStorage.removeItem(DEVICE_ID_KEY);
  } catch (error) {
    console.warn('DeviceId silme hatası:', error);
  }
};

/**
 * Basit UUID v4 generator (fallback için)
 */
function generateUUIDv4(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}
