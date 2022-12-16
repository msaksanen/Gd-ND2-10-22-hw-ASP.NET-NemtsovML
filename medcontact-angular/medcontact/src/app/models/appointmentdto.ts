import { Customerdatadto } from "./customerdatadto";
import { Daytimetabledto } from "./daytimetabledto";

export class Appointmentdto {
  id?: string;
  creationDate?: Date;
  startTime?: Date;
  endTime?: Date;
  dayTimeTableId?: string;
  DdyTimeTable?:  Daytimetabledto;
  customerDataId?: string;
  customerData?: Customerdatadto;
}
