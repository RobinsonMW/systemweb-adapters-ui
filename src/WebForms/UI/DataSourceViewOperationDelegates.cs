// MIT License.

namespace System.Web.UI;

using System.Collections;

public delegate void DataSourceViewSelectCallback(IEnumerable data);

// returns whether the exception was handled
public delegate bool DataSourceViewOperationCallback(int affectedRecords, Exception ex);
