import {User} from "./user";

export interface TodaySelection {
    id?: number,
    fullName?: string;
    userId: string;
    mains?: string,
    sideOrders?: string,
    salad?: string,
    user: User
}
