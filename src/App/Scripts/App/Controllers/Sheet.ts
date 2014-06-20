﻿/// <init-options route="/sheet/:year/:month"/>
/// <reference path="../../typings/linq/linq.d.ts"/>
/// <reference path="../DTO.generated.d.ts"/>
/// <reference path="../DTOEnum.generated.ts"/>
/// <reference path="../Factories/ResourceFactory.ts"/>
/// <reference path="../Services/CalculationService.ts"/>

module FinancialApp {
    'use strict';

    export module DTO {
        export interface ISheetEntry {
            category: ICategory;   
            editMode: boolean;
            isBusy: boolean;
        }

        export interface ISheet {
            totalSavings: () => number;
            totalBank: () => number;
        }
    }

    export interface ISheetScope {
        date: Moment;
        isLoaded: boolean;
        sheet: DTO.ISheet;
        categories: DTO.ICategoryListing[];

        // copy enum
            // ReSharper disable once InconsistentNaming
        AccountType: DTO.AccountType;
        saveEntry: (entry: DTO.ISheetEntry) => void
        deleteEntry: (entry: DTO.ISheetEntry) => void
        addEntry: () => void;
    }

    export interface ISheetRouteParams extends ng.route.IRouteParamsService {
        year: string;
        month: string;
    }

    export class SheetController {
        static $inject = ["$scope", "$routeParams", "$location", "sheetResource", "sheetEntryResource", "categoryResource", "calculation"];

        private isCategoriesLoaded = false;
        private isSheetLoaded = false;

        constructor(private $scope: ISheetScope,
                            $routeParams: ISheetRouteParams,
                    private $location: ng.ILocationService,
                    private sheetResource: Factories.ISheetWebResourceClass,
                    private sheetEntryResource: Factories.IWebResourceClass<DTO.ISheetEntry>,
                    categoryResource: ng.resource.IResourceClass<DTO.ICategoryListing>,
                    private calculation : Services.CalculationService) {

            var year = parseInt($routeParams.year, 10);
            var month = parseInt($routeParams.month, 10);

            $scope.date = moment([year, month - 1 /* zero offset */]);
            $scope.isLoaded = false;
            $scope.AccountType = <any> DTO.AccountType; // we need to copy the enum itself, or we won't be able to refer to it in the view

            // bail out if invalid date is provided (we can do this without checking with the server)
            if (!$scope.date.isValid()) {
                $location.path("/archive");
                return;
            }

            // get data
            $scope.sheet = sheetResource.getByDate({ year: year, month: month }, (data) => {
                this.signalSheetsLoaded(data);
            }, () => $location.path("/archive"));

            $scope.categories = categoryResource.query(() => {
                this.signalCategoriesLoaded();
            });

            $scope.saveEntry = (entry) => this.saveEntry(entry);
            $scope.deleteEntry = (entry) => this.deleteEntry(entry);
            $scope.addEntry = () => this.addEntry();
        }

        private signalSheetsLoaded(sheet : DTO.ISheet) {
            this.isSheetLoaded = true;

            sheet.totalSavings = () => this.calculation.calculateTotal(sheet, FinancialApp.DTO.AccountType.SavingsAccount);
            sheet.totalBank = () => this.calculation.calculateTotal(sheet, FinancialApp.DTO.AccountType.BankAccount);

            this.setLoadedBit(sheet);
        }

        private signalCategoriesLoaded() {
            this.isCategoriesLoaded = true;
            this.setLoadedBit(this.$scope.sheet);
        }

        private setLoadedBit(sheetData: DTO.ISheet) {
            this.$scope.isLoaded = this.isCategoriesLoaded && this.isSheetLoaded;

            if (!sheetData || !sheetData.entries) {
                return;
            }

            for (var i = 0; i < sheetData.entries.length; i++) {
                var entry = sheetData.entries[i];
                entry.category = Enumerable.From(this.$scope.categories).FirstOrDefault(c => entry.categoryId === c.id);
            }
        }

        private saveEntry(entry: DTO.ISheetEntry) {
            entry.editMode = false;
            entry.isBusy = true;

            // santize
            entry.categoryId = entry.category.id;

            if (!(entry.id > 0)) {
                this.saveAsNewEntry(entry);
                return;
            }

            var params = {
                sheetId: this.$scope.sheet.id,
                id: entry.id
            };

            var res = <ng.resource.IResource<any>> <any> this.sheetEntryResource.update(params, entry);
            res.$promise.then(() => {
                entry.isBusy = false;
                this.$scope.sheet.updateTimestamp = moment();
            });
            res.$promise['catch'](() => {
                entry.isBusy = false;
                entry.editMode = true;
            });
        }

        private saveAsNewEntry(entry: DTO.ISheetEntry) {
            var params = {
                sheetId: this.$scope.sheet.id,
            };

            var res = <ng.resource.IResource<any>> <any> this.sheetEntryResource.save(params, entry);
            res.$promise.then((data) => {
                entry.id = data.id;
                entry.isBusy = false;
                this.$scope.sheet.updateTimestamp = moment();
            });
            res.$promise['catch'](() => {
                entry.isBusy = false;
                entry.editMode = true;
            });
        }

        private deleteEntry(entry: DTO.ISheetEntry) {
            entry.isBusy = true;
            entry.editMode = false;

            // if the entry has not been saved, we can delete it right away
            if (entry.id == 0) {
                this.$scope.sheet.entries.remove(entry);
                return;
            }

            // server-side delete
            var params = {
                sheetId: this.$scope.sheet.id,
                id: entry.id
            };

            this.sheetEntryResource['delete'](params,
                () => {
                    this.$scope.sheet.entries.remove(entry);
                    this.$scope.sheet.updateTimestamp = moment();
                },
                () => entry.isBusy = false);
        }

        private addEntry(): void {
            var newEntry: DTO.ISheetEntry = {
                id: 0,
                account: FinancialApp.DTO.AccountType.BankAccount,
                categoryId: null,
                category: null,
                createTimestamp: moment(),
                delta: 0,
                remark: null,
                source: null,
                editMode: true,
                updateTimestamp: moment(),
                isBusy: false
            };

            this.$scope.sheet.entries.push(newEntry);
        }
    }
}