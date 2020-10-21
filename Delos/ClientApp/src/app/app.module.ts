import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { KorisnikComponent } from './korisnik/korisnik.component';
import { NgbModule, NgbDateNativeAdapter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PartnerComponent, FilterPartnerPipe } from './partner/partner.component';
import { PartnerDetailsComponent } from './partner-details/partner-details.component';
import { NgbdModalConfirm, NgbdModalFocus } from './modal-focus/modal-focus.component';
import { TokenInterceptorService } from './auth/token.interceptor';
import { LoginComponent } from './login/login.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { PonudaComponent } from './ponuda/ponuda.component';
import { PonudaDetailsComponent } from './ponuda-details/ponuda-details.component';
import { PonudaFilterPipe } from './pipes/ponudaFilterPipe';
import { NoCommaPipe } from './pipes/noCommaPipe';
import { PonudaDokumentComponent } from './ponuda-dokument/ponuda-dokument.component';
import { WebShopServisComponent } from './webShopServis/webShopServis.component';
import { ArtikalComponent } from './artikal/artikal.component';
import { ArtikalFlterPipe } from './pipes/artikalFlterPipe';
import { ExcelService } from '../services/export-excel-service';
import { KategorijaComponent } from './kategorija/kategorija.component';
import { PrijavaComponent } from './prijava/prijava.component';
import { PrijavaDetailsComponent } from './prijava-details/prijava-details.component';
import { korisnikDetailsComponent } from './korisnik-details/korisnik-details.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    KorisnikComponent,
    korisnikDetailsComponent,
    PonudaComponent,
    PonudaDetailsComponent,
    PartnerComponent,
    PartnerDetailsComponent,
    PonudaFilterPipe,
    FilterPartnerPipe,
    NgbdModalConfirm,
    LoginComponent,
    NgbdModalFocus,
    NoCommaPipe,
    NavMenuComponent,
    PonudaDokumentComponent,
    WebShopServisComponent,
    ArtikalComponent,
    ArtikalFlterPipe,
    KategorijaComponent,
    PrijavaComponent,
    PrijavaDetailsComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(), // ToastrModule added
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'korisnici', component: KorisnikComponent },
      { path: 'ponude', component: PonudaComponent },
      { path: 'login', component: LoginComponent },
      { path: 'partneri', component: PartnerComponent },
      { path: 'servisi', component: WebShopServisComponent },
      { path: 'artikli', component: ArtikalComponent },
      { path: 'kategorije', component: KategorijaComponent },
      { path: 'prijave', component: PrijavaComponent }
    ])
  ],
  entryComponents: [korisnikDetailsComponent, PrijavaDetailsComponent,PonudaDetailsComponent, PartnerDetailsComponent, NgbdModalConfirm, NavMenuComponent, PonudaDokumentComponent],
  providers: [ExcelService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }, { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptorService, multi: true }],

  bootstrap: [AppComponent]
})
export class AppModule { }


