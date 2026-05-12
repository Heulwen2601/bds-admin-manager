import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, shareReplay, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category, CreateCategoryRequest, UpdateCategoryRequest, ApiResponse } from '../../models';

@Injectable({
  providedIn: 'root',
})
export class CategoryApiService {
  private apiUrl = `${environment.apiBaseUrl}/categories`;
  private categoriesRequest$?: Observable<ApiResponse<Category[]>>;

  constructor(private http: HttpClient) {}

  getAll(forceRefresh = false): Observable<ApiResponse<Category[]>> {
    if (!this.categoriesRequest$ || forceRefresh) {
      this.categoriesRequest$ = this.http.get<ApiResponse<Category[]>>(this.apiUrl).pipe(
        shareReplay({ bufferSize: 1, refCount: true }),
        catchError((error) => {
          this.categoriesRequest$ = undefined;
          return throwError(() => error);
        }),
      );
    }

    return this.categoriesRequest$;
  }

  getById(id: string): Observable<ApiResponse<Category>> {
    return this.http.get<ApiResponse<Category>>(`${this.apiUrl}/${id}`);
  }

  create(category: CreateCategoryRequest): Observable<ApiResponse<Category>> {
    this.categoriesRequest$ = undefined;
    return this.http.post<ApiResponse<Category>>(this.apiUrl, category);
  }

  update(id: string, category: UpdateCategoryRequest): Observable<ApiResponse<Category>> {
    this.categoriesRequest$ = undefined;
    return this.http.put<ApiResponse<Category>>(`${this.apiUrl}/${id}`, category);
  }

  delete(id: string): Observable<ApiResponse<void>> {
    this.categoriesRequest$ = undefined;
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }
}
