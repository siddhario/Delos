import { Component, Inject } from '@angular/core';
import { AuthenticationService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { Korisnik } from '../model/korisnik';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
})
export class HomeComponent {
    currentUser: Korisnik;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService, @Inject('BASE_URL') public baseUrl: string
    ) {
        this.authenticationService.currentUser.subscribe(x => {
            this.currentUser = x;
            if (this.currentUser == null)
                this.router.navigate(['/login']);
        });
    }

    logout() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);
    }

    ponude() {
        this.router.navigateByUrl('/ponude');
    }
    partneri() {
        this.router.navigateByUrl('/partneri');
    }
    korisnici() {
        this.router.navigateByUrl('/korisnici');
    }
    ugovori() {
        this.router.navigateByUrl('/ugovori');
    }
    servisneprijave() {
        this.router.navigateByUrl('/servisneprijave');
    }
    izvjestaji() {
        this.router.navigateByUrl('/izvjestaji');
    }
}
