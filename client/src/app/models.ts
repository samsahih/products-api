export interface TokenResponse {
  accessToken: string;
  tokenType: string;
  expiresAt: string;
}

export interface Product {
  id: string;
  name: string;
  color: string;
  price: number;
}
