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
import { useState, useEffect, ChangeEvent, MouseEvent, SyntheticEvent } from 'react'
import { Box, Button, Link, TablePagination } from '@mui/material'
import CreateForm from './create-form'

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
    const fetchCategoryData = async () => {
      const data = await getCategories({ pageIndex: 0, pageSize: 10 })
      setCategoryList(data)
    }
    fetchCategoryData()
  }, [])

  const [page, setPage] = useState<number>(0)
  const [rowsPerPage, setRowsPerPage] = useState<number>(10)
  const [openCreateForm, setOpenCreateForm] = useState<boolean>(false)

  const handleChangePage = async (event: MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    setPage(newPage)
    const data = await getCategories({ pageIndex: newPage, pageSize: rowsPerPage })
    setCategoryList(data)
  }

  const handleChangeRowsPerPage = async (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
    const data = await getCategories({ pageIndex: 0, pageSize: parseInt(event.target.value, 10) })
    setCategoryList(data)
  }

  const onSubmitCategory = async (name: string) => {
    const res = await api.post('api/category', { name: name })
    const data = await getCategories({ pageIndex: page, pageSize: rowsPerPage })
    setOpenCreateForm(false)
    setCategoryList(data)
  }

  return (
    <>
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label='simple table'>
          <TableHead>
            <Box sx={{ display: 'flex', alignItems: 'end' }}>
              <Button variant='outlined' onClick={() => setOpenCreateForm(true)}>
                Yeni Kayıt
              </Button>
            </Box>

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
        <TablePagination
          component='div'
          count={categoryList?.count || 0}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={handleChangeRowsPerPage}
        />
      </TableContainer>
      {openCreateForm ? (
        <CreateForm
          onSubmitCategory={(e: string) => onSubmitCategory(e)}
          handleOpen={() => setOpenCreateForm(true)}
          handleClose={() => setOpenCreateForm(false)}
        />
      ) : (
        <></>
      )}
    </>
  )
}
export default Categories
