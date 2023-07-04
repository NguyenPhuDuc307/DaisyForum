import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError, map, switchMap } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Pagination, Category } from '../models';
import { Observable } from 'rxjs';
import { UtilitiesService } from './utilities.service';
import { TreeNode } from 'primeng/api';
import { of } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class CategoriesService extends BaseService {
    private _sharedHeaders = new HttpHeaders();
    constructor(private http: HttpClient, private utilitiesService: UtilitiesService) {
        super();
        this._sharedHeaders = this._sharedHeaders.set('Content-Type', 'application/json');
    }
    add(entity: Category) {
        return this.http.post(`${environment.apiUrl}/api/categories`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    update(id: number, entity: Category) {
        return this.http.put(`${environment.apiUrl}/api/categories/${id}`, JSON.stringify(entity), { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getDetail(id) {
        return this.http.get<Category>(`${environment.apiUrl}/api/categories/${id}`, { headers: this._sharedHeaders })
            .pipe(catchError(this.handleError));
    }

    getAllPaging(filter: string, pageIndex: number, pageSize: number): Observable<TreeNode[]> {
        const params = new HttpParams()
            .set('pageIndex', pageIndex.toString())
            .set('pageSize', pageSize.toString())
            .set('filter', filter);
        return this.http.get<Category[]>(`${environment.apiUrl}/api/categories/filter`, { params, headers: this._sharedHeaders })
            .pipe(
                map((response: Category[]) => {
                    return response;
                }),
                catchError(this.handleError)
            );
    }

    delete(id) {
        return this.http.delete(environment.apiUrl + '/api/categories/' + id, { headers: this._sharedHeaders })
            .pipe(
                catchError(this.handleError)
            );
    }

    getAll() {
        return this.http.get<Category[]>(`${environment.apiUrl}/api/categories`, { headers: this._sharedHeaders })
            .pipe(map((response: Category[]) => {
                return response;
            }), catchError(this.handleError));
    }
}

function buildTree(categories: Category[]): Category[] {
    const map = new Map<number, Category>();
    const roots: Category[] = [];

    categories.forEach(category => {
        map.set(category.id, category);
        category.children = [];
    });

    categories.forEach(category => {
        if (category.parentId !== undefined && map.has(category.parentId)) {
            const parent = map.get(category.parentId);
            parent.children.push(category);
        } else {
            roots.push(category);
        }
    });

    return roots;
}