import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Korisnik } from '../model/korisnik';


@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private currentUserSubject: BehaviorSubject<Korisnik>;
    public currentUser: Observable<Korisnik>;
    private url: string;

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string,) {
        this.currentUserSubject = new BehaviorSubject<Korisnik>(JSON.parse(localStorage.getItem('currentUser')));
        this.currentUser = this.currentUserSubject.asObservable();
        this.url = baseUrl;
    }

    public get currentUserValue(): Korisnik {
        return this.currentUserSubject.value;
    }

    login(username: string, password: string) {
        return this.http.post<any>(`${this.url}korisnik/authenticate`, { username, password })
          .pipe(
            tap(user => {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('currentUser', JSON.stringify(user));
                this.currentUserSubject.next(user);
                return user;
            },
              error => {                
                return error;
              }
            ));
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }
}
