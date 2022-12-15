import { Selectitem } from "src/app/models/selectitem";

export class Doctorfulldata {
 id?: string;
 userId?: string;
 username?: string;
 surname?: string;
 midName?: string;
 email?: string;
 birthDate?: Date;
 gender?: string;
 isBlocked?: boolean;
 isFullBlocked?: boolean;
 forDeletion?: boolean;
 specialityId?: string;
 specialityName?: string;
 specNameReserved?: string;
 roles?: Array<Selectitem>;


}
