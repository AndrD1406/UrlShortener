import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable } from 'rxjs';
import {Url} from '../../models/url/url';
import {UrlCreate} from '../../models/UrlCreate/url-create';

@Injectable({
  providedIn: 'root'
})
export class UrlService {

  constructor(private http: HttpClient) { }
  private apiUrl = 'https://localhost:7135/api/url';

  getAll(): Observable<any[]> {
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", `Bearer ${localStorage['token']}`);
    return this.http.get<any[]>(`${this.apiUrl}`, { headers: headers });
  }

  getById(id: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", `Bearer ${localStorage['token']}`);
    return this.http.get<any>(`${this.apiUrl}/${id}`, { headers: headers });
  }

  create(urlCreate: UrlCreate): Observable<any> {
    let headers = new HttpHeaders();
    console.log(urlCreate);
    headers = headers.set("Authorization", `Bearer ${localStorage['token']}`);
    return this.http.post<any>(`${this.apiUrl}`, urlCreate, { headers: headers });
  }

  update(id: string, urlDto: Url): Observable<void> {
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", `Bearer ${localStorage['token']}`);
    return this.http.put<void>(`${this.apiUrl}/${id}`, urlDto, { headers: headers });
  }

  delete(id: string): Observable<void> {
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", `Bearer ${localStorage['token']}`);
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: headers });
  }
}
