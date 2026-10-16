export interface ZReportLine {
  productName: string;
  quantitySold: number;
  revenue: number;
  profit: number;
}

export interface ZReport {
  storeName: string;
  businessDate: string;
  generatedAtUtc: string;

  sales: {
    transactionCount: number;
    itemsSold: number;
    grossSales: number;
    averageSale: number;
    costOfGoods: number;
    grossProfit: number;
    marginPercent: number;
  };

  /** What should be in the drawer at closing. */
  cash: {
    cashSales: number;
    collectionsReceived: number;
    expectedInDrawer: number;
  };

  credit: {
    newCreditIssued: number;
    collectionsReceived: number;
    netCreditChange: number;
    outstandingTotal: number;
  };

  topSellers: ZReportLine[];
}
