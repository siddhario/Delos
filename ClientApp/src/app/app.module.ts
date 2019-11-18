import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { KorisnikComponent } from './korisnik/korisnik.component';
import { PonudaComponent, FilterPipe } from './ponuda/ponuda.component';
import { PonudaDetailsComponent, NoCommaPipe } from './ponuda-details/ponuda-details.component';
import { NgbModule, NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PartnerComponent, FilterPartnerPipe } from './partner/partner.component';
import { PartnerDetailsComponent } from './partner-details/partner-details.component';
import { NgbdModalConfirm, NgbdModalFocus } from './modal-focus/modal-focus.component';
import { TokenInterceptorService } from './auth/token.interceptor';
import { LoginComponent } from './login/login.component';
import { registerLocaleData } from '@angular/common';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        KorisnikComponent,
        PonudaComponent,
        PonudaDetailsComponent,
        PartnerComponent,
        PartnerDetailsComponent,
        FilterPipe,
        FilterPartnerPipe,
        NgbdModalConfirm,
        LoginComponent,
        NgbdModalFocus,
        NoCommaPipe
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        NgbModule,
        BrowserAnimationsModule, // required animations module
        ToastrModule.forRoot(), // ToastrModule added
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'korisnici', component: KorisnikComponent },
            { path: 'ponude', component: PonudaComponent },
            { path: 'login', component: LoginComponent },
            { path: 'partneri', component: PartnerComponent },
        ])
    ],
    entryComponents: [PonudaDetailsComponent, PartnerDetailsComponent, NgbdModalConfirm],
    providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }, { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptorService, multi: true }],

    bootstrap: [AppComponent]
})
export class AppModule { }


