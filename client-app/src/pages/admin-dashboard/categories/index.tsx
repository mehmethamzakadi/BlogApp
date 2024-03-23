import Paper from '@mui/material/Paper'
import Table from '@mui/material/Table'
import TableRow from '@mui/material/TableRow'
import TableHead from '@mui/material/TableHead'
import TableBody from '@mui/material/TableBody'
import TableCell from '@mui/material/TableCell'
import TableContainer from '@mui/material/TableContainer'
import api from 'src/@core/utils/api'
import { Category } from 'src/types/category/category'
import { BaseListResponse } from 'src/types/common/baseListResponse'
import { BaseRequest } from 'src/types/common/baseRequest'
import { useState, useEffect } from 'react'

async function getCategories(params: BaseRequest) {
  try {
    const categoryList = await api.get<BaseListResponse<Category>>('api/category', {
      params: params
    })
    return categoryList.data
  } catch (error) {}
}

const Categories = () => {
  const [categoryList, setCategoryList] = useState<BaseListResponse<Category>>()

  useEffect(() => {
    // declare the data fetching function
    const fetchData = async () => {
      const data = await getCategories({ pageIndex: 0, pageSize: 10 })
      setCategoryList(data)
    }

    // call the function
    fetchData()
  }, [])

  const [page, setPage] = useState(2)
  const [rowsPerPage, setRowsPerPage] = useState(10)

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  return (
    <TableContainer component={Paper}>
      <Table sx={{ minWidth: 650 }} aria-label='simple table'>
        <TableHead>
          <TableRow>
            <TableCell>Id</TableCell>
            <TableCell>Adı</TableCell>
            <TableCell align='right'>İşlemler</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {categoryList?.items.map(category => (
            <TableRow
              key={category.id}
              sx={{
                '&:last-of-type td, &:last-of-type th': {
                  border: 0
                }
              }}
            >
              <TableCell component='th' scope='row'>
                {category.id}
              </TableCell>
              <TableCell component='th' scope='row'>
                {category.name}
              </TableCell>
              <TableCell align='right' component='th' scope='row'>
                İşlemler
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  )
}
export default Categories
