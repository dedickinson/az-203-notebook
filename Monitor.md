# Monitor

## App insights queries

Check app log:

	traces
	| project timestamp, cloud_RoleName, appName, operation_Name, message
	| where timestamp > ago(15m)
	| where cloud_RoleName =~ 'az-203-todo-function' and operation_Name == 'OverdueHandler'
	| order by timestamp desc
	| take 20

List invocations:

	requests
	| project timestamp, id, operation_Name, success, resultCode, duration, operation_Id, cloud_RoleName, invocationId=customDimensions['InvocationId']
	| where timestamp > ago(5m)
	| where cloud_RoleName =~ 'az-203-todo-function' and operation_Name == 'OverdueHandler'
	| order by timestamp desc
	| take 20
