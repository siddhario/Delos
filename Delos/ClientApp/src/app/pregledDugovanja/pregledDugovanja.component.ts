import { Component, Inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-pregled-dugovanja',
    templateUrl: './pregledDugovanja.component.html'
})
export class PregledDugovanjaComponent {
  datumOd: Date;
  datumDo: Date;

  constructor(public activeModal: NgbActiveModal, public http: HttpClient, @Inject('BASE_URL') public baseUrl: string) {
    
  }

  ok() {
    this.http.get(this.baseUrl + 'ugovor/pregledDugovanja?datumOd=' + this.datumOd + "&datumDo=" + this.datumDo
      , {
        responseType: 'arraybuffer'
      }
    ).subscribe(response => this.downLoadFile(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

    this.activeModal.close();
  }
  downLoadFile(data: any, type: string) {
    let blob = new Blob([data], { type: type });
    let url = window.URL.createObjectURL(blob);
    let pwa = window.open(url);
    if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
      alert('Please disable your Pop-up blocker and try again.');
    }
  }
  cancel() {
    this.activeModal.close();
  }
}
