from __future__ import annotations

import json
from typing import Any

import requests
from azure.data.tables import TableClient, TableServiceClient

from ToolsOptions import ToolsOptions


def query_the_web(query: str) -> str:
    response = requests.post(
        "https://api.tavily.com/search",
        headers={
            "Authorization": f"Bearer {ToolsOptions.tavily_api_key}",
            "Content-Type": "application/json",
        },
        json={
            "query": query,
            "include_answer": "advanced",
            "include_domains": ["https://starwars.fandom.com/"],
        },
        timeout=60,
    )
    response.raise_for_status()
    return json.dumps(response.json())


def _get_orders(service_client: TableServiceClient, order_number: int, customer_name: str) -> list[dict[str, Any]]:
    filters: list[str] = []
    if order_number > 0:
        filters.append(f"RowKey eq '{order_number}'")
    if customer_name.strip():
        filters.append(f"CustomerName eq '{customer_name.strip()}'")

    query = " and ".join(filters) if filters else None
    table = service_client.get_table_client("Orders")
    return [dict(entity) for entity in table.query_entities(query_filter=query)]


def _get_figurines(service_client: TableServiceClient, character_name: str) -> dict[str, dict[str, Any]]:
    filters: list[str] = []
    if character_name.strip():
        filters.append(f"Name eq '{character_name.strip()}'")

    query = " and ".join(filters) if filters else None
    table = service_client.get_table_client("Figurines")
    figurines = [dict(entity) for entity in table.query_entities(query_filter=query)]
    return {item["RowKey"]: item for item in figurines}


def get_star_wars_purchases(
    order_number: int = -1,
    character_name: str = "",
    customer_name: str = "",
) -> str:
    try:
        if order_number <= 0 and not character_name.strip() and not customer_name.strip():
            return json.dumps(
                {
                    "error": "At least one parameter is required: orderNumber, characterName, or customerName."
                }
            )

        service_client = TableServiceClient.from_connection_string(
            ToolsOptions.azure_storage_connection_string
        )

        orders = _get_orders(service_client, order_number, customer_name)
        figurines = _get_figurines(service_client, character_name)
        if not figurines and character_name.strip():
            return json.dumps({"error": f"No figurines found for character '{character_name}'."})

        order_fig_table: TableClient = service_client.get_table_client("OrderFigurines")
        results: list[dict[str, Any]] = []

        for order in orders:
            order_id = order["RowKey"]
            query = f"PartitionKey eq '{order_id}'"
            if character_name.strip():
                query += f" and FigurineName eq '{character_name}'"

            figurines_list: list[dict[str, Any]] = []
            for row in order_fig_table.query_entities(query_filter=query):
                row_dict = dict(row)
                figurine = figurines.get(row_dict["RowKey"])
                if not figurine:
                    continue
                figurines_list.append(
                    {
                        "FigurineId": row_dict["RowKey"],
                        "FigurineName": figurine.get("Name"),
                        "Price": figurine.get("Price"),
                        "Description": figurine.get("Description"),
                    }
                )

            if figurines_list:
                results.append(
                    {
                        "OrderId": order_id,
                        "CustomerId": order.get("CustomerID"),
                        "CustomerName": order.get("CustomerName"),
                        "TotalCost": order.get("TotalCost"),
                        "Figures": figurines_list,
                    }
                )

        return json.dumps(results)
    except Exception as ex:  # noqa: BLE001
        return json.dumps({"error": str(ex)})

