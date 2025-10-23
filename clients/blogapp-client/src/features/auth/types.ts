import { AuthUser } from '../../stores/auth-store';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse extends AuthUser {
  token: string;
}
