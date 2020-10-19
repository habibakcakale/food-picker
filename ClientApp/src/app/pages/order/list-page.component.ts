import {AfterViewInit, Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {Meal, MealType} from "../../models/order";

@Component({
    selector: 'app-list-page',
    templateUrl: './list-page.component.html',
    styleUrls: ['./list-page.component.scss']
})
export class ListPageComponent implements AfterViewInit, OnInit {
    foodTypes = {
        [MealType.Mains]: "Ana Yemek",
        [MealType.SideOrders]: "Ara Yemek",
        [MealType.Salad]: "Salata"
    }

    items: { [key: number]: Meal[] };

    constructor(private activatedRoute: ActivatedRoute, public client: HttpClient) {
    }

    ngOnInit() {
        this.items = (this.activatedRoute.snapshot.data.foodItems || []).reduce((prev, curr) => {
            if (!prev[curr.mealType])
                prev[curr.mealType] = []
            prev[curr.mealType].push(curr);
            return prev;
        }, {});
        [MealType.SideOrders, MealType.Salad, MealType.Mains].forEach(item => {
            if (!this.items[item]) {
                this.items[item] = []
            }
        })
    }

    ngAfterViewInit() {
    }

    getType(type: string) {
        return this.foodTypes[type];
    }

    addNewFood() {
    }

    saveFoods(target: HTMLInputElement, type) {
        if (!target.value)
            return;
        const newItem: Meal = {
            name: target.value,
            mealType: parseInt(type),
            id: 0
        };
        this.client.post<Meal[]>('meals', [newItem]).toPromise().then(res => {
            this.items[type].push(...res);
            target.value = "";
        })
    }

    removeFood(row: Meal) {
        if (row.id)
            this.client.delete(`meals/${row.id}`).toPromise().then(item => {
                const meals = this.items[row.mealType];
                meals.splice(meals.indexOf(row), 1);
            }).finally(console.log);
    }
}
