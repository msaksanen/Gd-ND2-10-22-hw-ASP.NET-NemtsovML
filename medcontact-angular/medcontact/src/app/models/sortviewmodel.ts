import { Sortstate } from "./sortstate";

export class Sortviewmodel {

  emailSort?: Sortstate;
  nameSort?: Sortstate;
  surnameSort?: Sortstate;
  birthDateSort?: Sortstate;
  dateSort?: Sortstate;
  lastLoginSort?: Sortstate;
  isFullBlockedSort?: Sortstate;
  isFamilyDependentSort?: Sortstate;
  isOnlineSort?: Sortstate;
  genderSort?: Sortstate;
  isBlockedSort?: Sortstate;
  isMarkedSort?: Sortstate;
  specialitySort?: Sortstate;
  current?: Sortstate;
  prevState?: Sortstate;
  up?: boolean;
}
