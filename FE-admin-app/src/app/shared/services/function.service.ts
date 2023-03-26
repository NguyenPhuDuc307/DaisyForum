import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class FunctionsService extends BaseService {
    constructor(private http: HttpClient) {
        super();
    }
}