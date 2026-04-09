import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Product, TokenResponse } from './models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private token: string | null = null;

  setToken(token: string | null): void {
    this.token = token;
  }

  private authHeaders(): HttpHeaders {
    let headers = new HttpHeaders();
    if (this.token) {
      headers = headers.set('Authorization', `Bearer ${this.token}`);
    }
    return headers;
  }

  fetchToken(): Observable<TokenResponse> {
    return this.http.post<TokenResponse>('/api/auth/token', {});
  }

  getProducts(colour?: string): Observable<Product[]> {
    let url = '/api/products';
    if (colour?.trim()) {
      url += `?colour=${encodeURIComponent(colour.trim())}`;
    }
    return this.http.get<Product[]>(url, { headers: this.authHeaders() });
  }

  create(body: { name: string; color: string; price: number }): Observable<Product> {
    return this.http.post<Product>('/api/products', body, { headers: this.authHeaders() });
  }
}
