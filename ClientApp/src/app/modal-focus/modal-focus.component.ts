import { Component } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'ngbd-modal-confirm',
    template: `
  <div class="modal-header">
    <h4 class="modal-title" id="modal-title">Potvrda</h4>
    <button type="button" class="close" aria-describedby="modal-title" (click)="modal.dismiss('Cross click')">
      <span aria-hidden="true">&times;</span>
    </button>
  </div>
  <div class="modal-body">
    <p>{{confirmText}}</p> 
  </div>
  <div class="modal-footer">
    <button type="button" class="btn btn-danger" (click)="modal.close('Ok click')">Da</button>

    <button type="button" class="btn btn-outline-secondary" (click)="modal.dismiss('cancel click')">Ne</button>
  </div>
  `
})
export class NgbdModalConfirm {
    public confirmText: string;
    constructor(public modal: NgbActiveModal) {
    }
}


const MODALS = {
    focusFirst: NgbdModalConfirm
};

@Component({
    selector: 'ngbd-modal-focus',
    templateUrl: './modal-focus.component.html'
})
export class NgbdModalFocus {
    withAutofocus = `<button type="button" ngbAutofocus class="btn btn-danger"
      (click)="modal.close('Ok click')">Ok</button>`;

    constructor(private _modalService: NgbModal) { }

    open(name: string) {
        this._modalService.open(MODALS[name]);
    }
}
