// ** React Imports
import { ChangeEvent, MouseEvent, useState, SyntheticEvent } from 'react'

// ** MUI Imports
import Box from '@mui/material/Box'
import Card from '@mui/material/Card'
import Grid from '@mui/material/Grid'
import Link from '@mui/material/Link'
import Button from '@mui/material/Button'
import TextField from '@mui/material/TextField'
import CardHeader from '@mui/material/CardHeader'
import InputLabel from '@mui/material/InputLabel'
import IconButton from '@mui/material/IconButton'
import Typography from '@mui/material/Typography'
import CardContent from '@mui/material/CardContent'
import FormControl from '@mui/material/FormControl'
import OutlinedInput from '@mui/material/OutlinedInput'
import InputAdornment from '@mui/material/InputAdornment'
import FormHelperText from '@mui/material/FormHelperText'

// ** Icons Imports
import EyeOutline from 'mdi-material-ui/EyeOutline'
import EyeOffOutline from 'mdi-material-ui/EyeOffOutline'
import { Modal } from '@mui/material'
import api from 'src/@core/utils/api'
import { BaseResponse } from 'src/types/common/baseResponse'

interface Props {
  handleOpen: () => void
  handleClose: () => void
  onSubmitCategory: (params: string) => Promise<void>
}

interface FormState {
  name: string
}

const style = {
  position: 'absolute' as 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  p: 4
}

const CreateForm = (props: Props) => {
  const [formState, setFormState] = useState<FormState>()

  const handleChange = (prop: keyof FormState) => (event: ChangeEvent<HTMLInputElement>) => {
    setFormState({ ...formState, [prop]: event.target.value })
  }

  return (
    <Modal
      open={true}
      onClose={props.handleClose}
      aria-labelledby='modal-modal-title'
      aria-describedby='modal-modal-description'
    >
      <Box sx={style}>
        <Card>
          <CardHeader title='Basic' titleTypographyProps={{ variant: 'h6' }} />
          <CardContent>
            <form
              onSubmit={async e => {
                e.preventDefault()
                props.onSubmitCategory(formState?.name!)
                // const res = await api.post('api/category', { name: formState?.name })
              }}
            >
              <Grid container spacing={5}>
                <Grid item xs={12}>
                  <TextField
                    fullWidth
                    label='Kategori Adı'
                    value={formState?.name}
                    onChange={handleChange('name')}
                    placeholder='...'
                  />
                </Grid>
                <Grid item xs={12}>
                  <Box
                    sx={{
                      gap: 5,
                      display: 'flex',
                      flexWrap: 'wrap',
                      alignItems: 'center',
                      justifyContent: 'space-between'
                    }}
                  >
                    <Button type='submit' variant='contained' size='large'>
                      Kaydet
                    </Button>
                  </Box>
                </Grid>
              </Grid>
            </form>
          </CardContent>
        </Card>
      </Box>
    </Modal>
  )
}

export default CreateForm
