import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Login } from '../../models/login/login';
import { Register } from '../../models/register/register';



const apiUrl: string = "https://localhost:7135/api/account/";

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  currentUserName: string | null = null;
  constructor(private httpClient: HttpClient) { }

  public postRegister(register: Register): Observable<any> {
    return this.httpClient.post<any>(`${apiUrl}register`, register);
  }

  public postLogin(login: Login): Observable<any> {
    return this.httpClient.post<any>(`${apiUrl}login`, login);
  }

  public getLogout(): Observable<string> {

    return this.httpClient.get<string>(`${apiUrl}logout`);
  }

  public postGenerateNewToken(): Observable<any> {
    var token = localStorage["token"];
    var refreshToken = localStorage["refreshToken"];

    return this.httpClient.post<any>(`${apiUrl}generate-new-jwt-token`, { token: token, refreshToken: refreshToken });
  }
}

