import { Appointmentdto } from "./appointmentdto";
import { Doctordatadto } from "./doctordatadto";

export class Daytimetabledto {
  id?:string;
  creationDate?:Date;
  date?:Date;
  startWorkTime?:Date;
  finishWorkTime?:Date;
  consultDuration?:number;
  totalTicketQty?:number;
  freeTicketQty?:number;
  doctorDataId?:string;
  doctorData?: Doctordatadto;
  appointments?: Array<Appointmentdto>;
};
