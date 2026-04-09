import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Product } from './models';
import { ProductService } from './product.service';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  private readonly api = inject(ProductService);

  /** Bumped when a token is stored so late 401s from older list requests do not overwrite the UI. */
  private loadGeneration = 0;

  readonly status = signal('');
  colourFilter = '';
  readonly products = signal<Product[]>([]);
  name = '';
  color = '';
  price: number | null = null;

  getToken(): void {
    this.status.set('Getting token…');
    this.api.fetchToken().subscribe({
      next: (t) => {
        this.api.setToken(t.accessToken);
        this.loadGeneration++;
        this.status.set('Token stored. You can load or create products.');
      },
      error: (err: unknown) => {
        if (err instanceof HttpErrorResponse) {
          this.status.set(
            `Could not get a token (HTTP ${err.status}). Is the API running and is the dev proxy OK?`,
          );
        } else {
          this.status.set('Could not get a token.');
        }
      },
    });
  }

  loadProducts(): void {
    const generation = ++this.loadGeneration;
    this.status.set('Loading products…');
    this.api.getProducts(this.colourFilter || undefined).subscribe({
      next: (p) => {
        if (generation !== this.loadGeneration) {
          return;
        }
        this.products.set(p);
        this.status.set(`Loaded ${p.length} product(s).`);
      },
      error: (err: unknown) => {
        if (generation !== this.loadGeneration) {
          return;
        }
        if (err instanceof HttpErrorResponse && err.status === 401) {
          this.status.set('Failed to load products (need a token first?).');
        } else if (err instanceof HttpErrorResponse) {
          this.status.set(`Could not load products (HTTP ${err.status}).`);
        } else {
          this.status.set('Could not load products.');
        }
      },
    });
  }

  addProduct(): void {
    if (this.price == null || !this.name.trim() || !this.color.trim()) {
      this.status.set('Name, color, and price are required.');
      return;
    }
    this.status.set('Creating product…');
    this.api
      .create({
        name: this.name.trim(),
        color: this.color.trim(),
        price: this.price,
      })
      .subscribe({
        next: () => {
          this.status.set('Product created.');
          this.loadProducts();
        },
        error: (err: unknown) => {
          if (err instanceof HttpErrorResponse && err.status === 401) {
            this.status.set('Create failed (need a token first?).');
          } else {
            this.status.set('Create failed (check token and API).');
          }
        },
      });
  }
}
