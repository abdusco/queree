﻿# Queree

Dynamic filtering extensions for `IQueryable` that allows API clients to specify 
[OData][odata] `$filter` , `$orderby`, `$skip`, `$top` query string filter parameters without bringing the whole OData library to the mix.

```c#
[HttpGet]
public IActionResult Index(Query query)
{
    return Ok(_dbContext.Users.ApplyQuery(query).ToList());
}
```


## Example

![](images/query_params.png)

Giving `"?$skip=1&$top=2&$filter=startswith(Name, 'j') or year(birthdate) ge 1970&$orderby=id desc"` will filter a list of actors down to this subset:

```json
[
  {
    "id": 7,
    "name": "Charlize Theron",
    "birthDate": "1986-01-01T00:00:00"
  },
  {
    "id": 6,
    "name": "Jennifer Lawrence",
    "birthDate": "1975-01-01T00:00:00"
  }
]
```

## Limitations

- `$orderby` parameter does not support multiple orders (first name, then date etc.). At the moment only a single ordering is possible.

## Thanks

This library is created thanks to a [great guide][guide] by [Evgeny Bychkov](https://twitter.com/bychkovea) and [StringToExpression library][lib] 

[odata]: https://www.odata.org/documentation/odata-version-2-0/uri-conventions/#_45_filter_system_query_option_filter_13
[guide]: http://codewithevgeny.com/web-api-odata/
[lib]: https://github.com/codecutout/StringToExpression