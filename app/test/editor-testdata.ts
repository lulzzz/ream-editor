export const cSharpTestDataExpectedResult = [
    [{
        title: 'int',
        rows: [42],
        columns: ['int'],
        columnTypes: ['System.Int32'],
    }],
    [{
        title: 'string',
        rows: ['foobar'],
        columns: ['string'],
        columnTypes: ['System.String'],
    }]
];

export const cSharpTestDataExpectedCodeChecks = [
{
    text: 'Identifier expected',
    logLevel: 'Error',
    line: 0,
    column: 4,
    endLine: 0,
    endColumn: 5 
},
{
    text: 'The variable \'x\' is assigned but its value is never used',
    logLevel: 'Warning',
    line: 0,
    column: 4,
    endLine: 0,
    endColumn: 5
}];

export const codecheckEditorTestData = [
{
    // we should see an error while paused after typing the "=" (~5 second gap), and none the second pause
    "output": "int x = 42;\n",
    "events": [
        {"from":{"line":0,"ch":0},"to":{"line":0,"ch":0},"text":["i"],"removed":[""],"origin":"+input","time":0.005000000000563887},
        {"from":{"line":0,"ch":1},"to":{"line":0,"ch":1},"text":["n"],"removed":[""],"origin":"+input","time":69.20000000000073},
        {"from":{"line":0,"ch":2},"to":{"line":0,"ch":2},"text":["t"],"removed":[""],"origin":"+input","time":186.01500000000033},
        {"from":{"line":0,"ch":3},"to":{"line":0,"ch":3},"text":[" "],"removed":[""],"origin":"+input","time":336.37000000000035},
        {"from":{"line":0,"ch":4},"to":{"line":0,"ch":4},"text":["x"],"removed":[""],"origin":"+input","time":457.37500000000045},
        {"from":{"line":0,"ch":5},"to":{"line":0,"ch":5},"text":[" "],"removed":[""],"origin":"+input","time":659.4050000000007},
        {"from":{"line":0,"ch":6},"to":{"line":0,"ch":6},"text":["="],"removed":[""],"origin":"+input","time":870.7500000000005},
        {"from":{"line":0,"ch":7},"to":{"line":0,"ch":7},"text":[" "],"removed":[""],"origin":"+input","time":979.4500000000007},
        {"from":{"line":0,"ch":8},"to":{"line":0,"ch":8},"text":["4"],"removed":[""],"origin":"+input","time":6001.935},
        {"from":{"line":0,"ch":9},"to":{"line":0,"ch":9},"text":["2"],"removed":[""],"origin":"+input","time":6082.325000000001},
        {"from":{"line":0,"ch":10},"to":{"line":0,"ch":10},"text":[";"],"removed":[""],"origin":"+input","time":6217.8150000000005},
        {"from":{"line":0,"ch":11},"to":{"line":0,"ch":11},"text":["",""],"removed":[""],"origin":"+input","time":8646.215}
]}
];

export const cSharpTestData = [
    {"output":"var x = 21;\nx*2\n","events":[{"from":{"line":0,"ch":0},"to":{"line":0,"ch":0},"text":["v"],"removed":[""],"origin":"+input","time":0},{"from":{"line":0,"ch":1},"to":{"line":0,"ch":1},"text":["a"],"removed":[""],"origin":"+input","time":281.0450000000001},{"from":{"line":0,"ch":2},"to":{"line":0,"ch":2},"text":["r"],"removed":[""],"origin":"+input","time":368.4699999999998},{"from":{"line":0,"ch":3},"to":{"line":0,"ch":3},"text":[" "],"removed":[""],"origin":"+input","time":496.09500000000025},{"from":{"line":0,"ch":4},"to":{"line":0,"ch":4},"text":["x"],"removed":[""],"origin":"+input","time":952.46},{"from":{"line":0,"ch":5},"to":{"line":0,"ch":5},"text":[" "],"removed":[""],"origin":"+input","time":1089.255},{"from":{"line":0,"ch":6},"to":{"line":0,"ch":6},"text":["="],"removed":[""],"origin":"+input","time":1301.13},{"from":{"line":0,"ch":7},"to":{"line":0,"ch":7},"text":[" "],"removed":[""],"origin":"+input","time":1396.7000000000003},{"from":{"line":0,"ch":8},"to":{"line":0,"ch":8},"text":["2"],"removed":[""],"origin":"+input","time":2226.0000000000005},{"from":{"line":0,"ch":9},"to":{"line":0,"ch":9},"text":["1"],"removed":[""],"origin":"+input","time":2320.1700000000005},{"from":{"line":0,"ch":10},"to":{"line":0,"ch":10},"text":[";"],"removed":[""],"origin":"+input","time":2637.605},{"from":{"line":0,"ch":11},"to":{"line":0,"ch":11},"text":["",""],"removed":[""],"origin":"+input","time":3154.0900000000006},{"from":{"line":1,"ch":0},"to":{"line":1,"ch":0},"text":["x"],"removed":[""],"origin":"+input","time":3737.645},{"from":{"line":1,"ch":1},"to":{"line":1,"ch":1},"text":["*"],"removed":[""],"origin":"+input","time":4653.720000000001},{"from":{"line":1,"ch":2},"to":{"line":1,"ch":2},"text":["2"],"removed":[""],"origin":"+input","time":5061.935000000001},{"from":{"line":1,"ch":3},"to":{"line":1,"ch":3},"text":["",""],"removed":[""],"origin":"+input","time":6090.575000000001}]},
{
    "output": "var someStr = \"foo\";\nsomeStr+\"bar\"",
    "events": [
        {"from":{"line":0,"ch":0},"to":{"line":0,"ch":0},"text":["v"],"removed":[""],"origin":"+input","time":0},
        {"from":{"line":0,"ch":1},"to":{"line":0,"ch":1},"text":["a"],"removed":[""],"origin":"+input","time":155.22000000000116},
        {"from":{"line":0,"ch":2},"to":{"line":0,"ch":2},"text":["r"],"removed":[""],"origin":"+input","time":241.0600000000013},
        {"from":{"line":0,"ch":3},"to":{"line":0,"ch":3},"text":[" "],"removed":[""],"origin":"+input","time":511.46500000000196},
        {"from":{"line":0,"ch":4},"to":{"line":0,"ch":4},"text":["s"],"removed":[""],"origin":"+input","time":1049.2000000000007},
        {"from":{"line":0,"ch":5},"to":{"line":0,"ch":5},"text":["o"],"removed":[""],"origin":"+input","time":1174.125},
        {"from":{"line":0,"ch":6},"to":{"line":0,"ch":6},"text":["m"],"removed":[""],"origin":"+input","time":1246.260000000002},
        {"from":{"line":0,"ch":7},"to":{"line":0,"ch":7},"text":["S"],"removed":[""],"origin":"+input","time":1576.5000000000018},
        {"from":{"line":0,"ch":7},"to":{"line":0,"ch":8},"text":[""],"removed":["S"],"origin":"+delete","time":2080.9000000000015},
        {"from":{"line":0,"ch":7},"to":{"line":0,"ch":7},"text":["e"],"removed":[""],"origin":"+input","time":2201.4000000000015},
        {"from":{"line":0,"ch":8},"to":{"line":0,"ch":8},"text":["S"],"removed":[""],"origin":"+input","time":2486.8999999999996},
        {"from":{"line":0,"ch":9},"to":{"line":0,"ch":9},"text":["t"],"removed":[""],"origin":"+input","time":2707.2699999999986},
        {"from":{"line":0,"ch":10},"to":{"line":0,"ch":10},"text":["r"],"removed":[""],"origin":"+input","time":2784.6500000000033},
        {"from":{"line":0,"ch":11},"to":{"line":0,"ch":11},"text":[" "],"removed":[""],"origin":"+input","time":2951.504999999999},
        {"from":{"line":0,"ch":12},"to":{"line":0,"ch":12},"text":["="],"removed":[""],"origin":"+input","time":3150.01},
        {"from":{"line":0,"ch":13},"to":{"line":0,"ch":13},"text":[" "],"removed":[""],"origin":"+input","time":3275.074999999999},
        {"from":{"line":0,"ch":14},"to":{"line":0,"ch":14},"text":["\""],"removed":[""],"origin":"+input","time":3785.9200000000037},
        {"from":{"line":0,"ch":15},"to":{"line":0,"ch":15},"text":["f"],"removed":[""],"origin":"+input","time":4117.340000000002},
        {"from":{"line":0,"ch":16},"to":{"line":0,"ch":16},"text":["o"],"removed":[""],"origin":"+input","time":4277.175000000001},
        {"from":{"line":0,"ch":17},"to":{"line":0,"ch":17},"text":["o"],"removed":[""],"origin":"+input","time":4455.060000000003},
        {"from":{"line":0,"ch":18},"to":{"line":0,"ch":18},"text":["\""],"removed":[""],"origin":"+input","time":4667.845000000003},
        {"from":{"line":0,"ch":19},"to":{"line":0,"ch":19},"text":[";"],"removed":[""],"origin":"+input","time":4836.000000000002},
        {"from":{"line":0,"ch":20},"to":{"line":0,"ch":20},"text":["",""],"removed":[""],"origin":"+input","time":5416.890000000001},
        {"from":{"line":1,"ch":0},"to":{"line":1,"ch":0},"text":["s"],"removed":[""],"origin":"+input","time":6526.6950000000015},
        {"from":{"line":1,"ch":1},"to":{"line":1,"ch":1},"text":["o"],"removed":[""],"origin":"+input","time":6821.990000000003},
        {"from":{"line":1,"ch":2},"to":{"line":1,"ch":2},"text":["m"],"removed":[""],"origin":"+input","time":6923.630000000003},
        {"from":{"line":1,"ch":3},"to":{"line":1,"ch":3},"text":["e"],"removed":[""],"origin":"+input","time":7001.700000000003},
        {"from":{"line":1,"ch":4},"to":{"line":1,"ch":4},"text":["S"],"removed":[""],"origin":"+input","time":7267.360000000002},
        {"from":{"line":1,"ch":5},"to":{"line":1,"ch":5},"text":["t"],"removed":[""],"origin":"+input","time":7505.115000000003},
        {"from":{"line":1,"ch":6},"to":{"line":1,"ch":6},"text":["r"],"removed":[""],"origin":"+input","time":7567.435000000003},
        {"from":{"line":1,"ch":7},"to":{"line":1,"ch":7},"text":["+"],"removed":[""],"origin":"+input","time":8389.185000000003},
        {"from":{"line":1,"ch":8},"to":{"line":1,"ch":8},"text":["\""],"removed":[""],"origin":"+input","time":8790.19},
        {"from":{"line":1,"ch":9},"to":{"line":1,"ch":9},"text":["x"],"removed":[""],"origin":"+input","time":9399.400000000003},
        {"from":{"line":1,"ch":9},"to":{"line":1,"ch":10},"text":[""],"removed":["x"],"origin":"+delete","time":10252.210000000001},
        {"from":{"line":1,"ch":9},"to":{"line":1,"ch":9},"text":["b"],"removed":[""],"origin":"+input","time":10599.270000000002},
        {"from":{"line":1,"ch":10},"to":{"line":1,"ch":10},"text":["a"],"removed":[""],"origin":"+input","time":10749.500000000002},
        {"from":{"line":1,"ch":11},"to":{"line":1,"ch":11},"text":["r"],"removed":[""],"origin":"+input","time":10795.485000000002},
        {"from":{"line":1,"ch":12},"to":{"line":1,"ch":12},"text":["\""],"removed":[""],"origin":"+input","time":11142.230000000005}
]}
];

export const randomTestData = [
    {"output":"y","events":[
        {"from":{"line":0,"ch":0},"to":{"line":0,"ch":0},"text":["x"],"removed":[""],"origin":"+input","time":0.004999999999881766},
        {"from":{"line":0,"ch":0},"to":{"line":0,"ch":1},"text":["y"],"removed":["x"],"origin":"+input","time":2916.135}]
    },
    {
        "output": "test",
        "events": [
            {"from":{"line":0,"ch":0},"to":{"line":0,"ch":0},"text":["t"],"removed":[""],"origin":"+input","time":0.010000000000104592},
            {"from":{"line":0,"ch":1},"to":{"line":0,"ch":1},"text":["e"],"removed":[""],"origin":"+input","time":416.255},
            {"from":{"line":0,"ch":2},"to":{"line":0,"ch":2},"text":["s"],"removed":[""],"origin":"+input","time":842.745},
            {"from":{"line":0,"ch":3},"to":{"line":0,"ch":3},"text":["t"],"removed":[""],"origin":"+input","time":1088.5600000000004},
            {"from":{"line":0,"ch":4},"to":{"line":0,"ch":4},"text":["",""],"removed":[""],"origin":"+input","time":1312.5549999999998},
            {"from":{"line":1,"ch":0},"to":{"line":1,"ch":0},"text":["h"],"removed":[""],"origin":"+input","time":1755.3899999999999},
            {"from":{"line":1,"ch":1},"to":{"line":1,"ch":1},"text":["e"],"removed":[""],"origin":"+input","time":1913.1},
            {"from":{"line":1,"ch":2},"to":{"line":1,"ch":2},"text":["s"],"removed":[""],"origin":"+input","time":2084.945},
            {"from":{"line":1,"ch":3},"to":{"line":1,"ch":3},"text":["t"],"removed":[""],"origin":"+input","time":2179.05},
            {"from":{"line":0,"ch":2},"to":{"line":1,"ch":2,"xRel":0},"text":[""],"removed":["st","he"],"origin":"+delete","time":4920.77}
    ]},
    {
        "output": "some initial\ngoes here\n",
        "events": [
            {"from":{"line":0,"ch":0},"to":{"line":0,"ch":0},"text":["s"],"removed":[""],"origin":"+input","time":0},
            {"from":{"line":0,"ch":1},"to":{"line":0,"ch":1},"text":["o"],"removed":[""],"origin":"+input","time":164.34500000000003},
            {"from":{"line":0,"ch":2},"to":{"line":0,"ch":2},"text":["m"],"removed":[""],"origin":"+input","time":258.19499999999994},
            {"from":{"line":0,"ch":3},"to":{"line":0,"ch":3},"text":["e"],"removed":[""],"origin":"+input","time":397.655},
            {"from":{"line":0,"ch":4},"to":{"line":0,"ch":4},"text":[" "],"removed":[""],"origin":"+input","time":517.5499999999995},
            {"from":{"line":0,"ch":5},"to":{"line":0,"ch":5},"text":["i"],"removed":[""],"origin":"+input","time":665.4600000000003},
            {"from":{"line":0,"ch":6},"to":{"line":0,"ch":6},"text":["n"],"removed":[""],"origin":"+input","time":743.8299999999997},
            {"from":{"line":0,"ch":7},"to":{"line":0,"ch":7},"text":["i"],"removed":[""],"origin":"+input","time":892.7099999999998},
            {"from":{"line":0,"ch":8},"to":{"line":0,"ch":8},"text":["t"],"removed":[""],"origin":"+input","time":930.5350000000001},
            {"from":{"line":0,"ch":9},"to":{"line":0,"ch":9},"text":["i"],"removed":[""],"origin":"+input","time":1087.8849999999995},
            {"from":{"line":0,"ch":10},"to":{"line":0,"ch":10},"text":["a"],"removed":[""],"origin":"+input","time":1195.3500000000001},
            {"from":{"line":0,"ch":11},"to":{"line":0,"ch":11},"text":["l"],"removed":[""],"origin":"+input","time":1331.8749999999998},
            {"from":{"line":0,"ch":12},"to":{"line":0,"ch":12},"text":[" "],"removed":[""],"origin":"+input","time":1490.4750000000001},
            {"from":{"line":0,"ch":13},"to":{"line":0,"ch":13},"text":["t"],"removed":[""],"origin":"+input","time":1616.265},
            {"from":{"line":0,"ch":13},"to":{"line":0,"ch":14},"text":[""],"removed":["t"],"origin":"+delete","time":2188.74},
            {"from":{"line":0,"ch":12},"to":{"line":0,"ch":13},"text":[""],"removed":[" "],"origin":"+delete","time":2377.505},
            {"from":{"line":0,"ch":12},"to":{"line":0,"ch":12},"text":["",""],"removed":[""],"origin":"+input","time":2719.4349999999995},
            {"from":{"line":1,"ch":0},"to":{"line":1,"ch":0},"text":["t"],"removed":[""],"origin":"+input","time":3089.55},
            {"from":{"line":1,"ch":1},"to":{"line":1,"ch":1},"text":["e"],"removed":[""],"origin":"+input","time":3167.135},
            {"from":{"line":1,"ch":2},"to":{"line":1,"ch":2},"text":["x"],"removed":[""],"origin":"+input","time":3410.99},
            {"from":{"line":1,"ch":3},"to":{"line":1,"ch":3},"text":["t"],"removed":[""],"origin":"+input","time":3602.9799999999996},
            {"from":{"line":1,"ch":4},"to":{"line":1,"ch":4},"text":["",""],"removed":[""],"origin":"+input","time":4017.455},
            {"from":{"line":2,"ch":0},"to":{"line":2,"ch":0},"text":["g"],"removed":[""],"origin":"+input","time":4323.57},
            {"from":{"line":2,"ch":1},"to":{"line":2,"ch":1},"text":["o"],"removed":[""],"origin":"+input","time":4426.685},
            {"from":{"line":2,"ch":2},"to":{"line":2,"ch":2},"text":["e"],"removed":[""],"origin":"+input","time":4661.775000000001},
            {"from":{"line":2,"ch":3},"to":{"line":2,"ch":3},"text":["s"],"removed":[""],"origin":"+input","time":4858.42},
            {"from":{"line":2,"ch":4},"to":{"line":2,"ch":4},"text":[" "],"removed":[""],"origin":"+input","time":4963.549999999999},
            {"from":{"line":2,"ch":5},"to":{"line":2,"ch":5},"text":["h"],"removed":[""],"origin":"+input","time":5095.055},
            {"from":{"line":2,"ch":6},"to":{"line":2,"ch":6},"text":["e"],"removed":[""],"origin":"+input","time":5180.665},
            {"from":{"line":2,"ch":7},"to":{"line":2,"ch":7},"text":["r"],"removed":[""],"origin":"+input","time":5274.735},
            {"from":{"line":2,"ch":8},"to":{"line":2,"ch":8},"text":["e"],"removed":[""],"origin":"+input","time":5368.845},
            {"from":{"line":2,"ch":9},"to":{"line":2,"ch":9},"text":["",""],"removed":[""],"origin":"+input","time":6030.1},
            {"from":{"line":1,"ch":0,"xRel":0},"to":{"line":1,"ch":4},"text":[""],"removed":["text"],"origin":"+delete","time":9039.635000000002},
            {"from":{"line":0,"ch":12},"to":{"line":1,"ch":0},"text":[""],"removed":["",""],"origin":"+delete","time":9404.6}
    ]}
];
