import {User} from "./user";

export interface Order {
    id: number;
    userId: string;
    user: User;
    orderItems: OrderItem[]
}

export interface OrderItem {
    id: number
    mealType: MealType,
    orderId: number;
    name: string;
}

export interface Meal {
    id: number,
    name: string;
    mealType: MealType;
}

export enum MealType {
    Mains,
    SideOrders,
    Salad
}
