import { Doctdataindexviewmodel } from 'src/app/models/doctdataindexviewmodel';
import { Doctorfulldata } from './../models/doctorfulldata';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-doctorpreview',
  templateUrl: './doctorpreview.component.html',
  styleUrls: ['./doctorpreview.component.scss']
})
export class DoctorpreviewComponent {
  // @Input() doctors?: Doctorfulldata[];
  @Input() doctors?: Doctdataindexviewmodel;
  @Output() DoctorFromPreview= new EventEmitter<Doctorfulldata>();


  onSelect(doctor: Doctorfulldata) {
    this.DoctorFromPreview?.emit(doctor);
  }

}
