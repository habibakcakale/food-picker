import {MealType, Order} from "../../models/order";
import {TodaySelection} from "../../models/today-selection";

export function mapOrderItem(order: Order): TodaySelection {
    return {
        id: order.id,
        fullName: order.fullName,
        userId: order.userId,
        mains: order.orderItems.filter(item => item.mealType == MealType.Mains).map(item => item.name).join(", "),
        salad: order.orderItems.filter(item => item.mealType == MealType.Salad).map(item => item.name).join(", "),
        sideOrders: order.orderItems.filter(item => item.mealType == MealType.SideOrders).map(item => item.name).join(", ")
    }
}
