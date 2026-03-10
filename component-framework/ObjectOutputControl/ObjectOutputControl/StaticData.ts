export const StaticDataSchema = {
	$schema: "http://json-schema.org/draft-04/schema#",
	type: "object",
	properties: {
		productId: {
			type: "integer",
		},
		loadCounter: {
			type: "integer",
		},
		productDetails: {
			type: "object",
			properties: {
				name: {
					type: "string",
				},
				price: {
					type: "number",
				},
			},
		},
		list: {
			type: "array",
			items: {
				type: "object",
				properties: {
					itemId: {
						type: "integer",
					},
					name: {
						type: "string",
					},
					value: {
						type: "integer",
					},
					active: {
						type: "boolean",
					},
				},
			},
		},
	},
};

export const StaticData = {
	productId: 10,
	loadCounter: 0,
	productDetails: {
		name: "Test Product",
		price: 100.23,
		misc: "this wouldn't show up in Canvas Apps since its not in the schema",
	},
	list: [
		{
			itemId: 1,
			name: "Item-1",
			value: 123,
			active: true,
			misc: "this wouldn't show up in Canvas Apps since its not in the schema",
		},
		{
			itemId: 2,
			name: "Item-2",
			value: 234,
			active: false,
		},
		{
			itemId: 3,
			name: "Item-3",
			value: 345,
			active: true,
		},
		{
			itemId: 4,
			name: "Item-4",
			value: null,
			active: false,
		},
	],
};
