# CVnHR

This project is commissioned by "Provincie Drenthe" and provides an interface between 
third party applications used in the province and the Dutch Chamber of Commerce (KvK).
The application allows for storage of records (inschrijvingen) and provides a simple search
service. Furthermore the application provides batchwise updating and synchronisation.

__Open Source__: The project is made open source to allow other provinces to use this 
application either stand-alone or in combination with the BRMO application by B3partners

Original development is done by QNH Consulting B.V.

__Contact__ Corné Hogerheijde @ QNH and/or B3Partners (info@b3partners.nl)


## Development environment setup

Get your development environment up and running by cloning this repo, get a government 
PKI certificate and arrange a subscription to the HR-Dataservice of the Dutch Chamber 
of Commerce (KvK).
- Run visual studio as an administrator
- Copy config files from Config/examples (in both QNH.Overheid.KernRegister.Beheer and 
  QNH.Overheid.KernRegister.BatchProcess projects) and fill out with necessary values
- Install the PKI certificate and fill out the values needed in the .config files
- Create a database on either Oracle or MS SQL and update .config files accordingly
- Surfs up dude!

# Documentation en Configuration

- Basic flow-diagram including digilevering and ESB
	- https://www.draw.io/?lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&title=CVnHR.xml#R3LzXkuNItiX6NW028zBt0OKR0CQkQRDq5Rq01hpff%2BGRkVWVlZmna05X1%2BmeNIsI0gmA8C3WXmu7I%2F%2BGss0ujkGfq12c1H9DoHj%2FG8r9DUFgHMeuP2Dk%2BDJCwZ8D2VjEnwf9OvAqzuRzEPocXYo4mb45cO66ei76bwejrm2TaP5mLBjHbvv2sLSrv%2F3WPsiS7wZeUVB%2FP%2BoU8Zx%2FGUVQlPj1AykpsvzrVxNfZxwGUZWN3dJ%2BfuHfEDT9%2BPfl4yb4erHPmU55EHfbb4ZQ%2Fm8oO3bd%2FOVVs7NJDYz71W5fzhN%2B8ukvNz4m7fxHTkC%2FnLAG9fI5d8PU7bvG3vlrmDN5zZL4z1udj6%2F2mbaiqYP2esdseTEnrz6IwEfbFQ7XWD439fUOvl7GxXg5qOha8GkyXffEfH5jMs7J%2FtO7hn%2BxxRVkSdck83hch3ye8EugfMYXCSFf3m%2B%2FeouEPo%2FJf%2BMoEqU%2Bo%2BQzQrJfrv2rka4Xn3b6sc2w7%2ByRxFf4fL7txjnvsq4Nav7XUeYjJBJwAehbA01zMM43ELPXQNt92PRjTCjADXwcXibzfHxmSbDM3TX067coXdd%2FXuuy3ni44KS%2F41%2Ffep%2FXSK%2FLsV3djR93fAVlQkTRx3eNXZX85pOYpEMI%2BuWTr%2FGP%2FOI5MNmf%2Bu0znqZuGaPPoxD6M4ODMUs%2BD6N%2B7N4xqYO5WL%2B9%2FI9c9XHqZbXg%2BM0BfVe08%2FSbKxtg4DdRQ34bNQhEfZsbvz8e%2Bi%2BPv158uYNfg%2BaXqfyhOMK%2Fy73vAgtASf%2FThPmEvyD8ejj0E4f8NJFg9NspojT0XSLB2I8SCfvn84j4x%2FP%2FNW3gf4w1QV1kAGfqJP3DOIP%2F2DzH16n%2FMWsQ0D9vDfKviIafTPfzQuj3zqd%2BMFv0T5gt9Y9ne51w1fs%2FUGP%2BO34nf2yIv2Dm9A9mLgR98d38r9uff18rfovVn8Xit8D%2BOfTVINE13eQaZ4Axiove3D4%2FaIo4rn9m2X%2FGfr8g5zeogn8XVj9KIuRPMO5XXvAb6zKmqv9n2fYfJCnxjW0x%2BDvjkv8q48L%2FBgULx7%2BdPv09RNM%2FmD8M%2FRkGQP6xAS5C34OX0VEXV%2Bka%2FzXwBf%2BEH%2F8lVvheM3zJMSgOLtcGF2T%2FR6Xbz2z5Fcu%2BDTj8%2Bxr5LwMz7H8%2B39BvZw9TP%2BCHyI8i7c8QWvAfYMj%2FVwwx7dr5U0ddMPrfTD3sxwb7KkWpP2agP4Mywt8zaE59XWexpvofloM%2FMer%2BbSB82pj%2B3sY%2FQrs%2FJQe%2Fp%2BV2Ms1FVrTZ%2F1Mm%2Fj8w%2FW2mf0%2Fa%2FmU2%2FgNi4F%2BPc78Twsj3vOpfCHQ%2FEgX%2F40D3kybN%2FwDQ%2FdIt%2Fbdruf3e0MlezO7nNcDrj%2Fbb30n8j3XPvmbCb1tlX2Ljr%2B6VEdDv%2FPPPtbqQ76WDULRBe4Xv5dVjmpPmPwxNf5Ic%2F%2BMF6wfI8W%2BeKvA3qYJ%2F36tO4BhPyB%2F1qmmCRAPiX9yr%2FqcT8A%2B77nth9R%2FKNf7rWov9vq%2BO0X9denyvqe7tFOVjUa7%2Fj5kZppHfUZq%2FjtL9O6xtIL9b2%2FiFnfyWseA%2FonQU%2FidY4A%2BsbvzzFvhZr%2FCvmeIfWLL4a9phX4z9D9c1vgYC9H0iED8wEvVnJMJfom1%2BNv9%2FGAbonxEGf7Z4%2BW9FwH8tVn6gZv9lYuVrtP3GHqzdSuZ%2FVm35mUF%2FvMKDfm%2FfH62f%2FRmlBf1LViH%2B4PT%2FgvVCFPnH8%2F2XLpWi%2F3it4V829%2B8Z8X%2FiYunPLPg%2FvVqK%2FoEFht9a9OteNaD64mDKf1GMf9yGX%2FlAs2dg6%2BDfv%2BzWQ%2F4%2Bb50THJ9Kk%2FnYNwj9HQPS7mNXIfF3HPuBhBy7OfjcXEZDfxKyfY25r22u77UJjv7AHX9CIUW%2FJ82fheM%2FdGXtH9Ay7He0DPsLI5%2F4HkP%2FvbskP2go%2Fpu1SX4iRf61fUr6x%2BuTf1bfEv0DEuf%2FPYT8B30F8luExAjyr4PIv0RO%2Fdfl%2BncrRfDXXSG%2Fmf4v5OfPlpPon621%2Flg8%2FKRi%2FhUzxr5XU%2FIq%2F2fVwZ%2FZ7%2FMMkvg78dt%2F5DfxhfzuU%2BL7DU8%2FucA3yfeTq%2FxTrvmREiNq4Ie4WL%2FxDzEs4IkGBrjp%2F3xa%2FAay8YvRf%2Fn8epV9%2Fv24ztQH7dcx5v4AqQb9r5cCTv369m%2FIdTTEAWvnyf%2F%2BeuJ177899zfDH7f2dfR3UfTfe64hCT6ea%2FgukP5QbPzfPeiAId%2BDzQ8fdIDpPyH3vlee%2FAsY%2B3992P5%2Ff2e%2BOgiT%2Bo%2BL0J9mXNjNc9d8cX9UtJnyoVY%2F6ujnCPN5BAdqXtEEWXL7%2FUU%2FRu3fXXkGPOvLR9e7oskuE0SXdr642vVKTILx%2F4MRar9%2B%2Ft63GbjXMfospwj5R12K%2Fdc%2B%2FR3tJb5vJRDwDzz6ZzgU%2BwO9hK%2F849NIvzhHAc41uqn4jPpfnPThdeaX556%2Becjk48mn7x38xQ3B1H95eCstdlCvPh35dRT6jaeA%2FrkQ48tbRAC%2BQdjCZnRzg2Qx627XP%2B31zvl3dr0qoevXfWNv3vWXmTl6TMEBrMvcHVe9XpH89Uvfb6Itb1h4vc5ufM0%2FbRNr9YN%2B4Vr9cu8J3hN%2BHTsrEcWqh3jb7VAZPqOs56PuhNu7NCMFUh%2FGwxdOuX9cOJTVoXco6VPqDfAud8yooB8Ou1dIfdvFIrg9sup2D9EbpuJZoskBuYzDMBXd6XrzeV%2F3k778KRTtY%2FFhS7u1J6esEl9u8%2BV8gUZzcudq94x23lR31RbemycMygUAIeWIDBVX8mUdxlWhKjzIySCNhV%2FiuVcoyJGuD8jgofgkiqXaE0R7OBOPJ7F06%2FQGZ0X9FbdCIsSTKyZk%2BIAIGH3VGJkri9qQM57GzzTxriMThewo4eZJ6EAlPprHUU4SSni%2BuYcexHT7uIIPuUBA1KP7s5R0%2Fn5JWCFXxIlULcFvy%2BvTJ9YvopxWYB1CELW%2BFD3lSYQMiR1GIXDJcusiwv%2FAdkElEd9g%2FDYVaqqeve1U730qJpiJlihx566QvWNHfh1oJwSBgrncp5B%2BOapFDAd8Xh8sr7M5o2HXJm%2FDjfKOj9p1lKbeMblMAz2CtrCREnnUaxJciCmWa0RrF1jh7Ov0NRFzmsjvJDUrdJ4Y7YqFnmOuTKMvtKXJjtGi4yFtSgb7o34k1ErrqhYjs5isjLS%2B6SFDpi7nkA3nnimOofnYwkSbj6sXgO16mrCjC3IESKIR0gkZe0M6JZjH2Cnjw6hdPU3b2g0FulrEazjVNRu5KC7D4i%2ByVBbFcVHIgJt0vdKCCRTuiQgUYUhN7IVXHF9jOULh3GEVdQpcITlQhP3qH0HhXW9NFuKgmiHtEDWrARq%2B6LGUoutFTZWav4Vkg85oYWB5i6aRcSfO56idPdIY13dJN1e4vgZ2xGlNE%2Bjgk8CA9oKS7CGZ3NeN3jCH5AZPCWiqbGoe8934PFZem%2Bh0ktnTUmcUISaDAIrtLE%2FJJwlhpi5mwEBeGAWkQNFOsqN1SDrzGu5S1AQQciO9%2BhmLK223Fy9mlABqXTGkaJPOCdUhR%2F0aVM9m8Su9Rx7XPaVd0oFrduT9NKZ83Zf0TRLpe8RbnIp9epuDbPJTcK32XO7ozC942qapA8wKYng%2FLOgCVEE2s%2FQBYVI3otY%2BpGt4ruARRGHNEFHEXVc2Xu71VilNMrHIdyKt6WZwlO%2BUCr21aGNOyVFdFEw417YviDKFYRSFsFVN4CBXrvsW3hm3NsLoO92jRmwdY%2BDW4iYtOin65s35yLttHV56n8PR98X5IbI0RTBf%2BilBC%2Bm%2FEugRX5dxCrR%2FG8gGz4dZ4%2FubMBM82XKHH8lDItAepJhKL%2BqknQIAhfkZj%2BSaFe0puk3iwil25ZLHgxiiI9p0AIGCWw6gSacmiCLKc6LhlGPNUk0Zz6jtSZdv8ZfKEEnbRY%2BcSFvKeKnX8cyRtC1ac9V1DcMoiRWN1ZNvezlhoBvwH85K1DhGZkRxpGtdRxGTJzVjm8Z0jLoIJQvzkdYP1AioElVdhaojc4NJH8yT18nExc0riHG7piQXZHH7IG5iAieuNNHGS4t6tCfIWltk%2FCiuU%2FY0d0J4JdQ4HFFtnx4pKkfd38Bmdseg6oJrOff1RjmDVNUG1WI8Vp7wBu7rTZmJOMbo3K7yOsEaBQU4wRnpnG00bEnBsN4jwiphm2wvYb4XrlQOVFmh7yEdI98%2BQHxdP4SE5vssvVdtyyMYnXcJXyaKapcucS%2BIWWpYop14CAZoPZHbnFmJj8vEgY1EZ4ebIsFKTU7TywgXTAzIQUvNhqEJkLpjabAf2MkoGLY2K1h4RRgbIBf3PFDHxRJK9AJJXG5p0O24ZE4iXPjNdVBPvaRxjKcJtQQSfhqiSy3LdUkTE1DFiF%2BPTV8jVqMFesa3FnNbOdzQgyYTtA0BJIXnvqNVs9zdnJ6diyexM6KiWtWG8HM5EbqSdmm5KsJZsryHb8xDPA9oNaltMvlYVExqbRG%2FBW5wqw7MJUmuxGKWS0IxpyenSLWq12QEisJo%2FtSthujbyJCg4l7IKSiS0grsiyrDDrHm6%2FVQTaPv5sl9d0TzjIICRPlCmI8dxbBatdQnRL1PPKqHWsnuNdS%2FodHwqgLh3%2FULjuINjY0cLrfz9bpKp%2FosJ1AmmeWqttytoJbn06pMfndipLAaxS5QUiuHmz6NcSzo%2FOquIQCP20wliuXeM1Gu4mlHbu0YoJy9v6Xl0DN856ejWgk87nUZj7Unwpk7RcCZwhwlO3HKqzGCoQV3nkzNcsWOwLnYldPuUUeg3FVpHPo3GSHUBnHaWxURIZQyLw%2Bz0MdczniOVixU9XF1ehbR0ZpqmtXsQ%2Fe5RwZSrhGNeqy%2BDCoDev06EEMCJh4IuYjWNochW8X1Y56BUwrSje%2BEK2FFuN7Tr%2FX9w5vkZh8XiBEQPZPKVU9M%2BMoxmwxm9z5GqFdGKLVaaArFvCngX0r7TQMvUkG6F4%2BDDtFVYZItREh%2B7JNyXFbD2FW8HJNfmQRabpERig2qIITw4M0IW9D9ZlapjqJ9eM67b5xw%2FCX4nQ8gB3h97wDRv95W3OdnzXUX%2FDN1vQEQysTC7APX02c%2FxIqE3C8iyLn7ElL2Mi4uAoc2KIQJS94L5SB5QLgvGBBIijipF%2B9LLbBbY%2B2gSGBPOZqIWxkdp%2BwEUKSSYc48uGFDQkl4CT2CUCnlsekQ7s5jhzVtPXKUVKyWW03ClTVB04RMPRKkJVE9RYOqota8shUNOfvimS5bR2jg6xFEFd7LW0wgrF7Sur9EkhE%2FgoW90yBv0o69k%2F4UvvsZM6BouMzriOoMrLEtLbYwqNuX687oexk%2B6Ae%2FCuQC63o6Fm%2BcyjLHp6LwntzyPE6KsTHU0ogbyRjDdX3Axa2utY5AOTGXrSeNUYgOks7Lby92daonieIscqwoPvvBC994rmtHn8P9K%2FzWsDvpkUD8GeSp0IYkVsiHrBprERLERUXCxT0Imnprpk37D1e72KIgYATygPh9o%2FXjMG4map7nzj89wkeFg%2BYZ0WyQ2MDTsK7jEp7j7UILAd553wC5CthoK5h3yOAuVMfc5wmuGQMbHsytHm0ZTyquuwlxy7LTHN9hlLRMbcSWasMHIvLJJXwJRGlelPAcAqsA8cOoDtpVJVdDpJ9oYWipEftYqOiqdAKTlajcXuzNPpu5kHophMa8Z%2B%2FPEASeyTRdf%2FI5T0LPi1R5HVJRr9U0Ck8wdy%2BGO0hdgr2q1rKVZmUfy9tqO%2BprgAzfroopI4fZIl8OJbovuqlwB41TQ0qKBmPGwO1BBGaJqm5evsp0TzoNmC1TvYjiBcg7%2FLRCujcEc94QxDmlD9JvQ5Kn8Vr8KNwHa8C4gBhtf9VqQKpXD2NJGPAEybPF5SrZSBZg9sZPYgQsuz4JkrpfHGTCezV9Eo8811QXmV%2Fvwy3qCfBVG5DoGUbO4GmJ9ZQqkmL2JmTQ64ug3aYjq9ZNVPeNl1LSuXtuqANugmJUnxZ5m1UtvDuYiDb3NkIBUKjo6aMPwLfew2Kg1ACDJ%2FMZ3hRNFq1T2tgjWt%2Bjs8DW7Gz3AJidGhtmzPd9ne%2FbQiF5nRL3RR1pghpaivhABfdt8sbaXtJAgPQEsMDEMPmzotfmZEP%2BTlh662GMANmEM8UnsPQb0q2LkA2wpGNJhRscatP53bk3%2BQrLrik9iATYSJZ6LGlQLEEPgyRIb%2FHXQ06kImxJY2fPHDBBodRN7gNYGPAHeiZrGttJajHACwqzAiVk2Mh2%2Fe3MxJeADqa5IRJxx08TdeD7DZiEe2jPeiK3Zx%2BI3rI8kWfscUlLv0%2FByLlofmnLkKp7SpkZiNUZUh62vbSP5wHXPAHdaZE3a0gUqmYcerYeXSRQhTYrH%2FF0pgxbzxcr2U8NIoi5J5RZeOmx2TcrhC8akCx9GqrxDiA3sOgnTgyg%2Bt1jnk%2BiM7mD2qsGNxPrINhxOX98Eip2KFrjmm6LJkPnDdZ801UxLp6Q%2ByS9caGNitUh8iSW4DVYD7%2FVKuWikIzPbeYTJxOsv3hTfNGnl%2B6%2FH%2FFp9KSU9d494qVIG9oeerNM2wi3Klmy7HkmBNM6r7hC3mW56iatDic8nGzyluD3i%2BBtgsAP6IgD7dKkAfuexiy0YckxehEPVb%2BU1zXBUa5Gs5quTaFcD3F4w%2B6olcp1q0J4USVGyEXz%2FFIiMhBXFkGYOp6u%2FRVdkLse%2FMVVqC%2BeLq%2FxtH5daj6U0%2BJir3OosAQMHuxilGPDKY0SgfdDjCTDZ5BUBopwT6AMnaMkX5Pqss3K0wUhJSVmz2SMvwrTovEyWEeDHjio4RlnpHggbdeAbMTO0aLo5GeEOHtIgRLZQVQv3FGt1bQnjBYvZWroQhMQHq8WUiEGyCvI542DhQ9KwMB5QCZ5OoeE0hY1tp3htoRBOBYU7z2P1o9vyDadiks0WudRr3FZ4u4AcIsNqfcSMUYyfdcIPTYCEmKLySe%2FBg10eoANMLVYSHMczKf64Ol%2BoClsH3YJJqZWWT19fjgjGiXoxXVCZKD04KmnntLBUuKyy8VXbX6zSMorD%2FzVUcQjeB0JESwLSOQjyjWoCQ15pfiP3jCxTruelIE5LLXjlHzxcJ9JURPzXY60%2FqZMJtIDBz21dkY71V%2FfIRR3TBp1T%2BHtopmMUIzkZPSMbPEqQlwt9k4TQ%2B%2FcgF5F64XHJVfjtQoAJaXGtCINPoxWKWFQmXRrGsd3oFpMk6CBCHybdWIXmg6S5QIKfDyiTpF6kcUDstff%2Bvscn2SAevRr2vlLUHEo7Y4lSaz19klFDsBiFnx9PKFaHE9pfyW6275sySxv2mm%2BOwjh7Ocm4wyTBtas0W0jPt8751ScS40gxsa8cpoRLl3eY8rEgU2po0L9Zu1rmz1R8x68V6mAW2cypah7ca9NfVnks7Bicnb7CBg1MElzLwuGtsoBNMC6DZH8Q9%2FkuKPGjI%2FZLsJJj%2B1wPxXh1bcZxV82RY%2FGUpCu%2BnTcVoaRFzuVi%2FoUkht3n7iupN7pkEmqI8x55B17LzKQjddo6Aa8lPKg30FotfzuBabSdzyvrAfTy2meDXm2G6%2FKhbCUaBMpM0oyDV7w8Az8jMTvdBiDKl4pNwy6VIT00f3dSwm%2BNamv6VvGQHzq3YitFY%2BgtnZuddPhPmU94YR0kAztdQK6k1hzP4yKJLGBdnKfrUExJZf6oKgPPdsyHu1sSwAEycASBu8ASioZ0ILJsk4EDCbANGf7oEYI1WJppbM%2FjoTa6y%2F0OM4qFJSZzhW6CkHRM9ixMUjg9KoDpFhrDercbEpiEAgcXdzWYL%2FJGvM%2But5PlIbRREh2BSh1B5GZXTnMOLq%2FNbenEZw3qTX9SgOVB9wTuyq6xlcJfk%2BcwHcaOHyzZhGsMMbsTSztMz%2FAFxFRwPGJLNNp3lVLU0McR%2FMRaedjf9H7GddtleVDOKoEhJyKG4y%2BBT2ykuCh0bEJ96XOFa6aY75lD1qz5NAyU7qO5edK%2BiH%2FwAIiCHphrQ0PMp7C8NhVhZwkLqmvwsHoqqW8JoMFXW4JKQ82mCawEsT0LrFrCWUuXYTWoWEyrUZ9QA4NPu2KQ6t9E5qSE0W64LySXSLErjGH1RbpOqSBzySXxMbm3lgnnaFUD9g8BVxM6PdCOGnsImKgizY%2BZfD%2FDTCm7wl8TvSe0Th1U50G8ZorCG2kFyhMZVs4i1sQZD0pkyQLTXxqdZgInmmUbilltxHi4qfA0BLVwePmUe5TU8WNzq1sWEFCe1X7YWdolNZx1TptJsDw641jewdCXmhskO%2FVatsrZwC2wBf2yMPMSt9qWeDgOaGKtizsO0LIEMr72HRfMp2KTEX1NxXety0P3YlRA%2BvQKUB4clBUZDoPORhulVxBpEO%2BvBuRGnWMZexE8nQdFaYi6NcavQG4exOGVFPSJNWAW5gB1AlRcR9kQ1ncCQqMOHXf98i9sw8ftMcjDjRPiijbyh0Oy%2BSOn35vhxXKX6Ji6A6qmF1ywLJxS7T4%2BaxBz6KXtiqtaSyhT%2FdZjfisFILGAy4EKmjng0SJxZiYd1zc7kGWKWcqOLILcZbe9dgJunDlCN0W31L7UI16AJZK56AVShqoKjMaHslFHIL5UE%2F1Klvnc6GE7kojkn6Bo%2FEG3mRBkFg8RhqbwFdDOJxSJJS%2BTrIRtt%2FXzSFoFOx0RhvWWPCJoxHj5diE66crFpbpcv3JBAO41ceNEKyxbXrrdeDKoK5PnnmJdzzLbMW8C75BQtAFY7F4UXjTYHk0nJ1XyCsyabhQxmITphsaN0oFWYdOMCX73ZxSKe7HlzpsVWmJyQdT38fFCAZLg4wHiVoOdDwKFKd8hF8iGGr8KhSm4U7ni7tfqliaFIq8TjLm6tQHR7r00AC4JfDY9TPAD7HYIcWGUeDmtURxPRxeNdC%2BQGMTRJIqjb2OxUJXF3o%2BlMcR8ikjvmSTbs9YLmem%2BMJ26j3kbUFY8XiK3R09ajgwqxm5NFFbV5a8ZEkelWVIxIm1GCakDfbT5Q7RYCuBVyfygweD9Hi5usQ9Lm26XkyGGFjebG0VBDB9z%2B3oQr5xxbW2hpyrXtTaaCFafM9Q2bvgk%2FMO3m%2FAaopzSxZqKGkrGVCEXve9E0eiV051obfx6PNijTld9ejRFHDQWMWyoIffmJYFAzDPat35xIg2wU9KObKTUA27tryrYQ067EPRIBf0lYpZ%2BwsT8HU5XkyaXVJDfnb83aHKkA8PihxjBAgkPHTudC%2FAmS3BoHFn77c47%2BT04sl2bnt0vBqaRLJpjgMpI%2FVs1meG5iuXDIRGX0NyaV%2FakrIWsnJ4XG0%2FtMJq8lY1hqBTAFY7rF0jcXgk3%2Fe8F93AiMLmTdfGCrIMVwAbeYNNAR9tfDNJ151LUqcs95gQgWlNB2ugt%2B8w0fR8FgHbE1K4F40y3w1gzV7Xiu21azbH7vEgZJWl7GXmXdw7hKnoZXuTc3LsMSEEXHuJooUtYV7qyrWi%2B%2BmIPT0Nst3eoIfOckK08MtrYANCGcXbRj7FlEKb1eM9%2BW0GX0JJEJs3ol%2FCwctVMy%2BFjHWdixbGfTOeEdzdGk99EY8FthwcW4tFKRzRUtBCSIgFeQPKZb0vNofA7MPIOpAygvOQ52cGOu%2BjI4j%2B0IHcXypYMy9FjsAKoS66F4Cyjjx67d4ciA%2FrFae0ECqaaG45mLw0zgD5AYHKbDqgh5liuOKGy%2BInUQDMWtiV7MU8BjEX3a8QE5HHqIOVD4%2B%2FUaFPPKStIBLrTGe7rX74ZA60fAMx6BpIYeuWh%2Biz677KnjPoJEkLF1ui1cDfAIsVULNy%2BJKwOCgNuyeB%2BlU6G41oXFlQvnFmey7p6t0sW5a%2F72fOytRO86lDohoegEb2O02Bsoq%2FREF3YfmqPL7Y2zbwdHw8EGXkMF1E38zRI%2BjM2QugLs%2FgSqAni0Iu7xLVHL1twHj0m%2FjRgBuSUBDwmUWj7S54LVmWVFQZ7lJKXeirbx1U1qMjcElo33j7Wn2wQYNRRDoGJHKxQuns8O3CqwmtsYk8KXsuTmsi2ILegnKAxEyKGga0WQVG5Cjdi3HWIpp3p1AV6CPnoG95IuhTYikanXpvMXbCUl%2FGTPSlu4PcXm9xM4MW4QU9V1V%2B%2BIgBsSYA7FoRA1B7SVk9NUyoEiKN5mW1dQV7K%2FXcEgJsC9Q9cS9txgnJxawGjpMlhMe8%2FTGKwQi5%2BO5mHSMhVz1WAS25Cpr89nuDY9y8uBD8ibZ1SbzCrKqYFLRoDoalMnnGtX6yhvLtN2Q65om%2B13e%2B8Ov4gpgvLcxapb%2B8OGPB1C3qDNMafoFGJV%2FCFnBk98V5dwpRDmtd%2FMGSCPNlpylUsiRN3uqwxEuf9tDI5RC3ISY3Wq90ifdkTUqD2OLwIUVBOirYNp7N6%2BjOrnYxB8ZftTlt2NMoreHk8me3xs%2FgRl74k2grVh96cra3KJ%2Br3QOxqTVF6XaPTWxySaxFz58pdtIQmotCRZ%2FqPZkbkNICg9tuM9WWJuYeZR5uN11Auc5PnpQC4dFhzSv0LYNsVxLwwRpw3jNtcO45ZkLdL%2BmDxxGIuO3Tqkp1jaaahFgMTvscSkUP6NYSh0Ttazi97fCO%2B%2FtistuKPyJRz9ssRHzjbiXl%2BzGe%2BQ0jHIbGRiWcwW4NRu%2FXnplqeVP4oG811WUc7UCgKcBzXKtOMTEN2AlR8cEgTgG94E3DWo2UZvruCnaryqBz%2F8IoKaBPQ9iSiwj6SKpcIhbE3kMOP8hMjU%2Fvc7FBgzZuxvFZgklK3FsmeXFgYX%2BbJyVI66dGzRGq1ADF6K4BiKQSKX1VmlFonBStTchL9rQQ3yNTMp5g9fX7Y6nxSZHpi%2BZ38F3iclDCxbQi5XgupoSZ0xO9k4nDgjuKGGoVXm57UZxnR83N2RhkduchwoJi7Z6uquP4a2ZfStkuQaJTcG2t4mNaSlAY4egutk%2Byyt8iwSUXUG6PM%2FXVici5FO9VKXOPfJK8uyXSO740D%2FztHdTsvl%2BzIatPKiaOqFS2Eab5bWOAMRwLgJLh9eedInOjEDPl7oZv%2F57A5oP3cIshq7bAP8ro%2BIh5lVS5AdWn0rnbhRz7PQ26KV5QyPv40QFlEc2S2lkUeR%2B1GD%2BCWh6OKwPX58Aoo3l32Scs%2Bo1xYHBVTbfl1Ika9Qd02fb7CTu3ODtLA7ErOAgsLOuuAnIGPLOXBEaFZLC0lRuwkbOdHfk0G2nfU%2FFFQpx7d20LegvQF3T1hShNKbJr4jZNoMxHYiU5TwaidErHQcfWaemN1kMfv8gbtUpfMhn0YB70lyu46bENy3AWVVVG9IZohmkCbXAoIpNHQCbAxcEskytX9gH64CKn%2B6oYRE%2F7oxNssW0twueWibXcCHjd6%2FMtvarEsG8g5LscprZICpA9FANjEkgAj8qpbHGpQ1bweiSOkdtl8aZjLRW715ebeuHiB4MYpI99Oo%2B2PdAEtBdEaPO3Bd1ir5L9ZyJE2KakM5qdxmhgmWIWUlJvFo0VXyBtde0qCNjt4MmmIm2VeJDOC%2FDPNu6JUycBsKMQFLjd%2FjLMAjVwHOgPZN0jWHBRaDC2j2VAlIt8CWf5X%2Fc7pGzNgqasYJx6w5nw4uezOAztSXwx8X7y4WLjwMi4R9b0lJ5PERRhpQS1kLzRFGhN83gzT4TKr2Kh0HOK39SLvlLduNNnw7fQ3JT86wSGxKySMWVrDTJUYBqauQTXcVdAuAZ1TT7gjQ6TYC64%2Fa0iZCbQxdO10EpD0Ux5%2BQ%2B3I8%2FV4GuELMjRHcj8UKoMUaaGNQmhkSOqUsOngRplvwzCcTjwJXxipo6PjjzmqRPkxDoi9LoC3edp%2BxJFpdakZ2KCeNiEuZulHJ7nBxnkY6LOB3xlM3Mz0%2FxlKf202BaQe%2FD7SR1r%2BpFg74gZCuuW4QKpY%2FeNylj6VWZKSaXlSzApRnm%2FyBkAM%2BW3uKCMmr7RJrcw1SaHwIjCzZ5S5SKARyNfrKpTY%2FjYEcItAQ2VCQPv%2BmTu2lmbH%2Bxa3q96k1xMSypPKd9txRpt5I7ChLyei2MBh%2FuG%2Bbaq84PlgsqngM3rQlGrPkYSGpce7AYYo3%2BAI0KbUrS0SPhAT%2BLYSAvLtU7cv7j5EuLgbKbcaTKj5SIt6Lx6uSLGiXjNS9h7M957AaU6oCY77OmzPbULSWKFqede3YUYI61NQ9MaT%2BKKD7y72ydP7qUQFu9qjMXGfI9vR%2BsLoITvIsdumnmSd3BbA2bbjXIwdgygOasd4JzM3kf3qubeoaZC79iRg9BVu%2FCoBq3tBgCa5jSyI4VbB5lN%2FRb3btYSpbkVDhutvuhTWeYZESnv86DTg7%2F6D%2FJt%2BmH8xtQY9JTQIwpVXVeyhBSEA7HM5DVguSKGj9ehoG5jNjew6yOYxjRVu22ahBMxG1%2FOjc60KoBHexG9vdO8%2BD2rvejHPYsQKRE4d3zbjtKlvr1MB6JIVlW8RQhoCKt%2BBCEiYqWSWNacxbPh7M%2B9kwAFc70xbqfrRy3Hk8SdjxViqgXQAXMnH21RvBXGGG%2BoD9Gqa7yBrS5iAAzYGkdxsD1dJC5O5%2BEr10we9rtn3%2BbNFpZuoS6R9XimC3WZZuFFNImiRMp5ogGF%2Be7ZD0TPqUuvQ4794OmSyaZp7RtbXUSGfY7elk1vC72rnVSEPCvWGI5oeYu%2BJ%2BxhRCJD0WRF0AxuSBMoVLHMCKZ1n9wmqN1nqaeOPUVgRUQXkoclqeI9pryFeDxgrB6K1sT4pheWnHryOnV%2FIcjNsUVPeYxj4L5vCY4evFbPK6p3T4rySGV%2F35r7u4SPZyfGox%2F6xM7BtBFIjsDqbgM92vPi7sDcEqB%2F0nqfgk3MIAMWDcir8maGzDcDN0WGHql4zPRbgm8lQYP81rl7U7ZIYzUZjzvBxPrJG6%2FQ9uWZSKsg%2BKPZfXOIVhQqvPzucquq6ndas3LaW7QO9FrMaWxvrRdYWQUQlDAgewGp7RLTkSSHanaht50aymQm079kTrtIr%2B9nRbDekEclpVLRR0M9XZLhJh2id0CaK7F7NYsZybEdKFvU%2FcH4m0Gbgl%2FuQ%2FEkh5wRJpEJWDzKVoLKJ4Ga%2BXPvpq23NnYJlCSSyO4w6KHf9enAX8jb7uO8wojFBW6ZtwDy5twUKlUgp6m0%2FK44QJYOfl%2BSm0p5uXlUl673bIhPG12F0oCFwS4mhCoxF8iXF6J9tMdMhHQ7alXmFC4HRn2IULoFVwaiix%2B9K1A%2BMLBGdsmYeBExaUPMS9Zyau%2B%2BO%2F%2B4BALUMKBSC820HsumXqwqMMLF0A4F57lcBumIPrAmKys9bBI2l0EzMsYtj6X9MxPIMOmyCxqTjYIw862i7fGx%2FnCqpoDTquReKmENEZ3XlDDf0nyv0uqXnTg5q1HyG2wCOdizGA6pkfpOUCTtpN%2FKhPvOvVBAjWRYfW%2F2BV9dAqYeDrHURq4fLiGFOBRKkztRZs9LkC6eE7NVvk%2BMw5hzYFb0hZzitk5PSXsWvZR3RIyWLI2DVUGDdUU9mB8wqSgr2L5gjwJ9X4RUTkXQ83nbFDPvbZnQB38ekHIBAc0z1KLPhCoOblm6EYpDArc9d2OyESrkyfEZbaG9r8ybNhgd9MNDgiFit%2BUpz93NuMwa1%2BBF8QyvIspg4U5ZSjnwrcN17NnGGBkGXtdYqpdhM%2FY4ARXlJg8e3i5340%2BkZR4m7OIKgji639xigh7kA7n5JPPchFSzrgo04Jdkicbn%2BhJhZctPj9TIHgQKx6NiyogRSu6Jw0DAOJ7egsa2iMTCKtWjnbxqCBWd%2BQjECajGl6gi1Uwa8DDcV05mR4nU7%2FicrM%2BmrDmrl%2BEydd743b5NypoSAIncK4OAPEhopcTBI3vCI7VCI5mseLLsi8qt9yg0vHiE5YvnUO%2FHMuKkQod6G63GFNgU74XpXXg5pSZkOebYZfciS6LYlJMVEqGOkH5ClFknFjLtSf3l1pMO3VL9ZrnRrSgIXEHhEpoFN%2BHXCMqtkkOdopP3QH8o3fJWgrdElZ0h%2BPNOZrkUy6AsrWYixgDU9xE509wJZuFSPzfy2RSqb876AjV0DHb3qa6JCkQHGXErV%2BcDaux4vrSjUT5fRB1QRU3gdZcgfH1eAiexe7vqTi8A9TtTTj3HXxV0MkoqRq8oonjqdmNvt9fbZGwB5UNzVOjz9WoXgxNnepDGHDmU8wG6b%2FMF6VBqqeH0us%2BlxBYMHJBPC2Hs6T6jW5WtyqEJNTLVXEhna3gjvGyJRZmZ8ajWiEspgwahvtSWIkYxidLzR5tELZBRRys2fLUEnOnbM9e8ECXnyVxHmLyT8VtZwppAF7WYy1q0uQZEzaPx88hW0uxJ9%2BRVFP3BK%2BJmRlg41ZpT5WBdJu%2BwebEU3fhCb8vsFENa3Sa8quC7sECj%2BsrTlz7vDUAyNLxRCosRa2gK0cAByAoyGtJQGawSwKQhCjexOs9F1xt9YdDg9rlfYZG5d4WPy03XetJRBkN4RSUX8JmzMpp7fzEj%2FMpfLNoEIrvn41RMWgvp%2B5gonQI7OhxaF%2Ba3jCzWEjsnCkH14qJcoYa4PgatJ%2B0sXsVm6t6BnjRkWrsSt%2BfbTVYlvXvT63yvJn1JPh%2BmHISjmXENfDkt5LkX7dQrH4pYqSlyzm6GhFdNYaeHB2M7vdZFRlMZJmTbVFTjRT%2FfaYw3V35ge1%2FcfG9NwCMifRek2z7ID26UDblEkG5es61dcR85ylKjXp3dOonIcONd3spM85dwMpewukAgAd25vtAbDLTVhEB%2FQ8jeNtGSs0vbIkt4EsVOLXuhW5BYDoevIyM2tsPZEFDuFAxeUdvxHNP1NCOBe7xkA0YtuLkPY0811iWnVr5FvdvoSEIFo3sDuHdEX2ko1O0jdQ%2B3uOTGZMJ0ri4NfkdVU57f%2FtQ8wuVBiXZBGFdaCSDhOFMu7vELLvk6sd8wKgl4NSq9hM0C2rigK%2B1Xmj7lZuKzL1zTq55oYUMWKQ88rsIYQghhFkLk4eAYk4VadUGdQUpNOv2O41rv7vMs2wDKsmKhJsa1vJ1fkWKBF1YZbW1%2F2xl90G1MlOLwqN5Od6J6fuhLhLwIoHtbOH3OC1J5OSxHz9t5pFz%2FkljsKqpYcburm0kf2sfmWkeYNai4gWhH50I3NBMaSrpwjWrXqWBnG8FO71ALg3bFmaWP%2BWl1ntGyOltNou0UScx4D%2Bfubp11nN2kCt6xAgtB9aA%2FNITRMkIzA%2BRjeyrsA0Ff7S6otDZv8lKLws%2BzIg0ipZqLKp6zt%2B%2BQ23x0XD916FVo75iHqkp4U55rhbGQjh3GWTThSeG6JgpuC62hAUv4yWNA2DIe%2FuLa51tzyIkX%2BtFivDWX%2B2lq0NnHe7AjTVCI%2B07o3jzXS0GXsEvO6gN%2BoPwJp8PpiPSuB27BrgJuUDoU3OEYYZQn7Ht0c1%2FQBqVrGBvhAJC1gZlorCvmokxLRMmj4R5QmsBi6KRmECJYzp5vLuZ7hPsQFSj2hXipBRRmVzmYifLi85qpbFJCNu51byg8tCE0P0T6TUJP9yaWL5YGjYrnDrFpoFOdj0iaUkVFqL2pQtfceWTXJuuDt6lqZYih4nvXbwZjFy3A%2FYkHPaq9MW5GKRb5KAM4hWvq2XpEPV9BVrjAGnednp2XgRTVgaqapatrhJQZxu1jOWbQg4bL8aCtUUpS4VjpSJoEkVV0oETAasDU6lD7FDjFQ7gpwc6R5wVSiwsyZUblqphkdom990oNPlzrYxDP5UIvAz4e8RJimpbhyGu1%2BHWnjF3IvaQPeao1zMUljydxJCk%2FIbWChIGVpkVxDyA0klr19iT7Q0oM3WLarhz8ZApAB%2FCFXlr4UPGLMeyvwRuxM1Ya7FA5CgkemqIwJpkYQCzNMOmqQbXZhI4gZ%2FSYgjzLVil2n3ROF5srYUZBJKg%2BFNTluVJHLnR%2BI2f%2BvGAEtPsJ8N81CPKGudp7IgzQv2G9iA1WDkmyiLZSQDal7Dm8jWx%2BN2abt26YxQ6%2ByFfoElx1O4ZHwstUeoFfpCNzmwz8yuliJ6F3wmbWWQqk%2BLkSizjE9v3L8gTBDSaTyhhyWwhYkUTZ3J8T1KeQWNF7M1L3mjCWHQZLf3xOsTOhw7YAlq9AALVlKDaUhy33qjSpeLtVHmgk4%2FpVLEaewLmO12MWoQQ0w13YsLlSglnrlAMJVMJnl8DF0wvNJI4QAWhdimpDNrwnyvo4xDi%2B3dM3brnYrb4Cn2BXxwydVIjH2AX7GZXhCGK6cnMR70cf6SwbUC6Rir3f7IkOsXcHOvF0bcNcvCHoVcvlMzSMj%2FZt6HXinXjO0kA57IHtOa4tVYUZokREPkMVSjcbC9qnhozgkRrFVFZ4C4aqS4zFvIbUTH%2Bi2l12WjHmPFbUZATN2MyLtPTJth68TgI9DdKwCtjmVPI7Mh4h%2BQKrGUQFQTIeDvMCCrxDmN1ePNFGZyxpJArkofHQOt8rshEX%2FFDSV%2FBgXTJ5U3fI68XHEp%2FqPl9cMERTGXaL%2ByKTckmGauzeyAFakI%2BllMF%2BqEbd6VB0fWmMzHf0FPeZzPqdCB87J0LJg3NWmDpaxAIbEfOFnlfFRnFCb3gaB5zoDmMsDfYMtHeaFELty6aYd9ETAo0T%2B%2Bhm93Vj1gYREcMGJSJqB7jYQ52Zp2lGGvSoqInft2ROWfAQAdTBYuzHOcI%2FYmaN9ss%2BK8wosd36kBSC8GYsQm3nWt2f%2BToKJ7yCZxqEvYKHM2pCsSTjJDi4Alk5KR2ucneP1GFr68QB0lR4%2BjLr1HOvt6E6eJ4v30a3j5yOBcrNEXm50aNwF%2BZFIpkybV%2BWD%2Fb0M9FrqQJJV%2BWG68er%2Fs6PDGzkFJDTCUqCaRG5ghvQfnUUegdU0bDsknbYsIHDgrFNZQ4AMKbHA7Db0dRhb0dvMj8Obi47thNKw812oW0hDY7cd8Mx0ockZuM%2BKXhkqRE5Lze8xECXfX%2BBPq6LXHV2detYI%2FflY6XpeD%2FiJoTnlhFDCZKIkBZfZKFFe5JrqxG4vVJjSajrWL3J9HwAkQstULW9CVxyl8kzzx2uWDsx121f8eZ94oDYSmj7qHKj4WfyPXBjJq6qix8socJueFdxs9aXY0O9DCV6dd%2FX0e5aQFHfJBO3i4qZs49MLXHMEz8jaEPSpS9H71VAtLXuZKg0S%2BY2AHHpaGuHzc%2BHb%2Fn65q8rP2vS7r%2BJcodnnLHeMojXI1pVSXy%2BJK3OdtZn3weH82h1EnbLbDELECjHq3v67CPDCuX3S560dC%2B9Z30bJbcn0hsv4i9mWV%2B7%2Fsodt0S1k3Oyq46Y3mrOVuIdBt%2FOYqmroHVFPWNjvh26x5nCYes8wN5ze99qDOxRYSpg9rHSG4EORKbtY261%2B7gj94nBQ4mMSzyObArz0jprkh0iOYIOC%2FQO%2Bhl7N8Qt7h1xShfSnB4GNVwirSYuZyvkWxY1gmcnjsLFgEOTUzvTrNgfwnsMify0hv4UwVcbd4M%2Fs0dhJ4EkqqbPTI36eoA7EzwMH18yckFfWkFuughoxG9WFCU4D0%2FBAS%2BqhZhcF4c4olfpYICVCFWJlYQ750bLMOiOBmzame%2BPHaCAHbdxFyL84qgYDnZDlN5wN4N2zkYko6pwl%2BXgyq8lQaJ3Zg9Ec37sVrktk%2FLcG%2F7Fz5U0Km0mscHLw7R5FtdsL95sraf0A27PIqFe%2Fcbbc7VVUvc6aqi93%2B2D1%2BJkHmhRlskEe2dxaQucRTVg9eIgwgPRqFLLTi8%2FQcw56fkozOCRRZsz8mdyW5h5BivOD%2F%2FK6fe0jpdsopUtImZ3EBe71skbFVYplCtPxMM8JLD9c50k62WxSGIwxyw5dhYS4%2FqEyI9VBQNcrUrxtxvG4RYcmG2FSFZqORc%2B%2FOrldBSq7tPU8ulIXbXoBeQYM2OyH4NFYPXJHH4CQ4SEba6T75fKi5lgYh7MHIp9w4ZZAhVgq%2FsF9ZScbewLspJdCQMsHy2whF9viwSy9rV0KJsRVzE94Yn0OGt4yBqStcO4z9yypONQ3M6imwsoJbSiK4RBR0DfwgsSyRADXAjwsfz2uaCrBmJvflGBkFqRrsQeQXiJkMABvBYZXkQAojZHz4%2BWm9F0UUjh0ApiDcXuQJTGlutFjUfzy0vDSf9yjwMfaTmPZlp3I743Z2LrRwkKsMJRly1HX6ZK%2B8IZoAFMLyjN4lA8IYHH%2FmCg5yGMAmo1c4uQje6U800%2FY5bm7sKMFnJAlzncyrRhN3YPs8uRCkpek%2B6LMcqr5ttXzZdFYSEE7wSYXyOLUJ9R3LDSHRgi8METbRnm3QOyX9AifC8FFwuRmrSiajv0R2NRjOiZYQJGvUojpwhVC%2FaACFCKoDWytjkujcW5X%2FoqPRH2%2BVgYdRGwnnpmNSJD5iMtEmWDt6gsnrNSniO5xy0mpxixp644Nyl5f5DJyr1idkimiUHbVs262kR3AslMl7zRT2UzVjZb8HFd9xPK9Res%2BsidHMtUTYiSkts3jhIT3d0OAuw1mJBtkicsZC%2FK%2BKSPyqoTU7j4DOEywi6DSv0k1sq8B2Vic8v0hi6l1mugkmTBOdwSGhZT%2BebEAqc%2FSEEQkc0bnxzMs2liOLj%2FdC%2Ft6fE54HRzgM5qOdAIHFcWM%2BVDZqYjvPWN0wpnsB79AyCGbxGk3IB9MywSqtQbcElDT2WVGgUameL846mh7eE26LoJlcSKWyr33LLedCNnBHhfyzfXTKtIz7wZP11jdvRy0uJLvG4xDh6HFwy8ETHSJGhjV2tkVBr2PaQYHkZCGkdVHiv2s%2B3ooU%2BPwYVI6cBBvHJbt90gR3sG2FE%2BqoXba9C8DFZdDe8YtDtp45SlYOPF6asxkvNp3nDEflVmHRWFtA6VZDQiaGwR%2F%2BWBRwgIf3SVXUgsQAjoUMt0An%2BOy4SsYAF0S86LMwP8QGHJnlHqIAQzlQ8788bqFoYK6JVYGXflC4%2FTpnyyzL5Ovvtqu5m1%2BGwJli10LFRSvWOCqyxSMEyHu7tQXeX91TxU%2FXbqDrvkODFkdh0MUpvUr447IX8t%2FEKDARthkJJJfOPltIr1gkOVF0ZrJ713VmC2bUc36o5kZPccV5WuyD44fFKcQrDRgxFmJejtxL2ZiQ86i62fmXUNPJe8eRc33lBi7NHsWwrNv9dVLVSs7nBvL7UgLvQu1qokFGkxvr%2BBVklzbTbhNbXmqHlczN4VhFg996FTgM6%2FuKQa92MQSCRgp4wdlcJbTy0BkScSK%2FjVzSKf9iNCDEqWLJUeUPw9y%2Bq63%2BDjKUSv8qO4khomb8T9uaOzwInKRotmzLqUGjOgqMv1OhQGu%2FTD%2BnDsGAZyMsQFkNy8CVFGS60H41hO3JfyeIOKFgfKayrgXu8k6XUnpTEpwaNUnUGO91OLcpok76fgy8mByarHcz3oNlBm3hsTC4ejL2J1Japv6L3u5O6%2BDa0vkhuF1S%2B6uqIW3FKx5wJS8wlL7bYUjcpjp%2FSBZCmrkp2tH%2BxRufCruB9DOBvtvUJrXog3Lhs9bpmvGkt%2B7CTqO7%2FNSkBj7w31vM09GcvjgbHKKVyzT5qW932AhshpIroLax9bV1gyx4TchF7SmJuO94aprt8%2B%2Bq9m2%2BWr9sV%2FWCy3F9%2BskHfHxVcUfPUe9433RiYiPE95NTlBgDbxAhmyIBgrei%2FTd1y8r8xWiWmJOc3M423bp7qrpwnKPDy2ZJ3VV2zH8adXZyrWlvxesIR%2FycPXmpedYaSDePR3SiabAWt8GNVIO0sSLMXvBfb82FLhKAd7zpP29puPlczEgioHfkBbRZntnRqrd4uK8%2BQiVnPJzvK%2Bqr%2FusgM9n%2F%2Bfre9YkhzIlfwlanGkSKqk1uSNWiS1Jr9%2BGdXz7I3t7q2tq7ssMwIBuAMOoHGthnrQPbfWXxuHDEqyMw6a4n9BIl6mquuTWhK5vu%2F%2F0XaRqdp%2Bh2O0R%2FPwQpu%2BEYhKO8BuT%2FTzwRs6WhJHmXsmH5MdIvzS0QslVvd7iuWHzoMjD2i%2B2NSt0b8fYlNtxMTs2sNSFZs2McCXudvRIJe%2BVJ1l5GtQF%2BxLUhE2ngXuUTRqwsMecPVlvTA7A%2F5yro1TIGOohQrJyhT53s0BPUqIZfPbSBaH8aIRIolmsJ4rIRz0brRSCkXan0hhOcz2UucQVRXzEYE%2BFSlfPnP%2Ba70TWmjoHwBkyopcyJ3%2BmEwY0qjdBnlsGuPZh%2B9njT5wRBgfzoh6Z1NNZ9Toa3o5hKUt4a0RmttUqcuKaN0pRg8opSjn21997nV5aWRG%2B%2BD%2FTcM4o9Lc1Uw3PnRu2mrKFZqLESayEmPznvuSw9KPg%2FL2prtMvYV9Q6%2BCOYax1XMVRzczW5IPxbV64UpEWyJA9C879%2FOc26ZVB61J9gz6wH5KerAVmicmQzcrfggRfowGWmJu9T7GYg029eOqnAEJYUJzxyQcnxUkJmJNW2M4seU%2BN6ANURK83VsLmiJYfHEGrxS7gNNusJzyg9zoNL24xH2KJivkGcOQZzVaXhoh%2BBAr6VteNK0xVxtiXowwR8p1BXmZrtxLDv%2F%2Bn%2FZP6r0EGXH52W0E411F0WjdxmR0Mif%2FQbAWCdcUwsOVjUgeMCV3dQhr%2BBMpUQ5FT9RfvSa8R908Tee1YnT7KzTI2%2FUbmD%2FBBC3lHfRbkZab8hiif7bPr4Q1XlU8Y%2BsJ4ViArW36zKLf4t9cVC9LX6rEg3nUuDFVj23gevLSHncNSTfIGYZVRB%2BskUy3NvPxkycnuUg5PGyE%2FXoSCYDpzRyVlQhr6hu9J5S7W7vmqcEW9LEbmxUciF1prm1O2b0I9WNyKs3%2FxtV1g1FTqg09dQMTNfi6eMzW%2Bm%2FOc4oBiMeS%2BOkDsgmFVNOHZmHQl1s6mvKLQTbyqUJm1ypcOr0xyEGAzsgCGpoNC7hS4W1NTGhsWOQzjfEmO9GUIb4NnffDGaQw2u2wFEJpTmSPtWOyTXZRijn90Nh7NrUp4uwB7359SOY%2FtykRCoR3iAZRFBjQMLQFcQvX1I%2BhYxeRAXQwXYB%2B0drWB5vDhyUZOLvPrh7%2BbFEueRs1UKuOrDTNKY8DK8Hrpls91Wt2cKfZnLS19CL9PiPONKgmfwn3bQXsbiH2akbfOmidxabOTyuml1i3JIFmkU80%2Ft8QqtaS4QhfmUe9aZF%2Fg15zlYfhCShntgz%2BmcG51K4f17vRqrsCIvJIk%2Byoc760KdMV1fClNbxwMVi3YGxsH0jJC5QwR2FvUxQuibcSrFtkdM83y0TJvnJF9UaVuJ1FAZrETfo1usjcS78jJSiGjoWwL9wCPI8%2Bf2E00A%2FmTwbnI58sEVjvBocoFcOzZ2aOQ5DQla7MFIRNFtsZJGngaEFCg%2BkI1ZJY%2BPBBbOp1EX1SsvERFgFZ%2Bbuq96SOM275GZ9H0yHZVR8lLIpbf1hPDRNfFGsjjigHVONYRtVNUz1wDN3XOkviWYB%2BYc%2BDx4P720UnzDNB1h4TyHT%2B4kErdVsNJBtWS5C1U%2Fl8MUuryaD19msCi57giawYIpKWIAx6QvJh8yDkyfqgAE91BdOeBaYjGykRZdT1V3%2F8vDROU9QrgFzi8%2Bxjeb30VclkOtmUCuY38bt5BpYaIgKxVKuIi%2F6evRVmMqHxn2JyC1Yz31PoBL90Y9oiRdad3Z9MzFdztXWNU4zMXbSMjuLDD0DZJz6U8v%2BoDpLM3KXr7tLlNVxqwET8hT30U2zdX%2FvOceZGdxbkpDTm%2Fjh2zbs0%2BrULn0GWz80Pth1s1ryEJ4r3Qtr2TNxZQZzaZZn%2F6JdKDuIlxBtb70dzvb90MArSOGPUZH8oMsdVrTqIDr2sjkt3U15Sh56uwfggkfSxHZcWby974WzTFnmuFWqvHGJSysyftFqopBelYr8k%2B%2BLuvfOB8YBwI7QXLyEmd0vltyv8GC%2FJI22dM9CE%2FR4t%2Bmgp%2F980ofjX%2F87rM3QJqXDGRn7p6oKY6KENd5GlrVnDPgi3roRli5MKMKGDBaSZyxcxOtE9Qkjrb2jL4YZSRb0443HHJupKh8nXuQX4ol03ooiIC3JfqsnSS9e6nuevukKv0wojy3NbA6bBcGV0X6hh91Hb8yLNkSMfEQoZOqam%2BOJx6b981KNRglx4Wl4Ia0Va0mI38svAE8yCIX4ZCI3dO5GYFv%2BvG1SIK0XZeASM72VfG7NnkcNTGf2UXuvGt3oDRcUHw02Zgr9%2BgsbBi7FMKWg6g%2BLhsc7dHj8KD1odumP%2B0jIQBJwLDQdMg30yGPOqT8YdKkt0myap4T54mQTan5Tmi0NYoBSXyjT1oOKDp8byMcgVTXle9kMu4ZEgvovxaX9h9UvHl6mo6ogVISch%2Fg2y6rFmoX%2FDzYgJHqGeY5PRobVL8BJ8N8InGNyzAt3Nwd853Y75fQ%2B2knk%2BsQjbcd7IAdPmpz0QZtM31higMpDX7VoqCJHIDtRL0SrrpmKVslSzcgTmXiNTt2xZ8jvNwWA4lWDzHSu0bBrD58VZVaUGP9LKaT%2FcPV38lFPrmtB2DeLQY1l0bzxKgY5%2B1sWEOd6XO%2Bv%2F4gLyJ8E3KMYub%2BV1lLo6SiTuxXBftEJo5n8887xCwDBvukoNJW2Rb0FBN5%2FhB7GGVwPkBapFn817ht95xuGvsvxwKUJnEN2hATS5y7m%2BMCr9wy8kn8RvUgwLjGv36zmm%2BqOEU%2BHo6xzfYdGK%2BtKM%2FeegSEnWoJlEk024doZ8j%2BE7FtlOaNsqCOk9aEdTW7TG01ixKmWxy3LiXa57%2FJh%2BdGLC%2BWCpP7C3Md960WbKIccZzsLnbyniCE3VO2BUTELr%2Fszme1H%2BSs9s2234Q31PMDNTkAkqrESxorXnfn0f7X3J1JZBkH8ixUY337TioCQxpmF0UlJaUIBg3Djz5mm2gE%2BNcYUCWUyxCoTvXEZ6O59PKytEwYOgL6VHfFSfoJbDJ7B%2Bn5d%2BUGm2yCqD4y8MG2NfgmvnAAZXd%2BCXv%2Fi4LvOVM%2FlTjlIAv%2FzIJDsVMeQ7jWh8rMU9W6Srybk3vLPNltPa61ac61B%2BruDjBU7lBbSESoL132LB%2F3mL5pcUYZ7PMX6lq%2FT6W90URXarGtdelwdNM890wcMqbHQGDtepWWVBgsHxTKNGE0cHel9p9%2FGMRaPq5b2dPomlUjW327XKTtz0CJX0OZR5yEfp9sDkDL69V4WqeaWPBqIVl%2F30Qj1d6Tyj4jce%2BJY1H3XQ3ydCGlUnFbBHsx0ovR4VJ%2FNoLLr5xdIyDMe74vROyt0f6svgHPF1wRO3oM5CFjFWFIcpTZzMXkyVHuLvNa2p9xROtWPTZ16%2FpPBa4aHuE1Jrk2CHx62kl4TJY%2By3p2%2Fa7n7HaaN4J7cRUb1J6z3kTrSnfftg4X7ufL1rNVVzWoZZ%2F7qnjtqpD%2Fx1%2FI14LF%2FweKp%2FdSmcg%2FsN%2BqGKsLj2DAsmbw6FK8bSkdmbT6%2BGnnjb%2FAFXMvoJo0o%2FVwTAp4kRAeQ%2BT%2F6XZgVO1s12V3AhYbe71i%2FK7yea7L9oG8CHWSbsDWgSrYX9tqvwBaymvkpFqPZsZ3F3KmvXCXGENuhoxykXoue%2FXplC8UJyFuXr%2BpuCFMQY4%2BbUVedqs%2FaWvYoG7CSxsq%2F3qbdw1uLZhV0k9Y2nRc05UJwoBli4xNIY%2Bt9IbhF61qYunwfhxGfJNgS8zUIhL2hPpH6VAaL6S3sYwK3P60oI09JKxiWqEsDSONFLBbpqsdN1XDz4PQ9cBkgCmO2QfwNhlPRf4hboiqxuXxMQLYWcGAGpk5HXBPHPgXBkMCdRS0YFskhpuz3mi%2Br9PAdGZKUFQ29EmcE4kEmpRk0W7poQOJn8De9xzNN6egN0hs3E3xC8%2BHOgjcC9QJfFf%2BjfxLN9Tv5y8qAwJruamUGiBh2%2FR6d1q2SOhNCDUcFSyt5TrW1IuR%2FWUKnFchKGITl9yyMieJI9UmEswUD598l5e9v3vnit8Bv042YL%2Frp3Uk4tvoUOV2Uo3cxwU8WkuTj7ab95ip2yFK5wA8rFMNXBksUzz3BzwJmpTbSiLYfTMnTG344tN4HalBCzb7HSSH%2Fafv0yWRPbkOFKSAReXxPUj1cqFdz8V2CkBi%2FOXIFQvgYZmJQ0mxB1HwJQkpdSa9MOArPHx0%2FJXJ8xEyY8KOIkqBxLPkAyW6rAJY0oSgfTfvwS62zv%2FUcJYcszW4lzL3MGRYoPwPopyJ1VEjGa1GdDXrhy1htsb%2FhX4ydzBDmPM%2FSg52FyEuQc0ChNSqOm54Zo5wWlWDIS9mN5hiOoieKewy3%2BXkva7mNIfUCFT%2BAMjECIdO%2BH5AcSz4YWqJwCSYiLSlC69IaOEf89kTTERqscGBAFsE5uCvr7r%2F2DxdqbG8CnhViUNoNCyp6at1XcyynK3AnzfnAxLMUxcahf3Xb%2BBRV%2FBa7lV1t7qcqcKvdrW%2Ff7h1I17imNHjBsx6WycwKT49kgra98nL1Syq6%2FEY1M1e7ukqrtwUqm1WhtP8BAxVSE1TYbM%2B4RtMovfVjAH3BrsrILQ%2FERqdhRnR7IkNXXX0tn78NYGMv%2BQdxlZFBKekUm0gy1Ge2mNcSrU88mr8p6s6OVW75fIcjvs6vC15Olf61mBVotM0kMJaa73q%2BFQVrDcYvGOLx8UfjyiSf0%2B3rpIRJRHBScC3pROT%2BHTnAABt5S9rcU8qSEy1zG69N0XjxSQc6u%2FUtpiVwyHXJrHjX9C6AOMCDTeAObp19fYM5mbOTsrspI4Lvxa3BztZEn8uRtashtu2dJvjZoSsSGJM5NCPnobJhcWJ6MvMUFxfWGoxdslIi5tP6otWRAFwUqdqk6lFNEKTW62fp232UVDx1%2FiF61xpNxj5lra8oAhM3ij2K%2Fkrk3zOEUVOBexjia%2FqmScHYjPMm0x9YlUMfjQ%2Bko4hsimD%2BTEe5iSFCMMPptNAmQX%2BpCbcNlOpyd4gSq5Xwy8r01lREht9cN8t1LnVSKJovgIxWtAgDHEAveifhIjvYDUF2yIol3m5FXeBrBKwMQFnqBNAsHXyVq0WX6VZ9PoOFYXi5fz%2BWp%2BLJkPJjsIg7%2Bsh8DvdNjR9XYoiOus1LwZJ8C0ttocM0n3bGVmzbWBJKrI5DaqrdXhzc1fvD39Tcgmcs69%2BAXkEzryxb96jT2F7%2FvifZ9PesM0uMn2X4%2FqqPQB73%2F4Dt8QKdNY5PIXBYTWedP4So5Hrp8vNoXfUZyjAEmKhDuoTIaTZFmbp8k2oVzoktIDMcf9W98sWf2YW62JIhtZx5i6fcRCoEHiaSXSgMqmzaP04uReKahvp9oGdEQg6k26HGyYI195GUJ1y6B2zuwQEU5LiNiXuD3FaSnRfAiRoFy%2FmzSlUcCH7%2B4WeMYtNtFXrBi4SJn6v%2FcSt5G1jhDq3TEVgPmhrJx6g4vdMiEFST52pqJjOyhXlQ4%2FWjkDXoyFZ76kA7N7RvCDyoWaDy0fl3rv%2FlaaiHeFD0k3VKvSQzZDUEiuY3HzwKxHztzdB31P1q5aJ0MFLtuKs7iFfWF8cs2JNxEwwUSfV95P5Of4PNU%2FO5ZJuDwUw957UBNCRBcOulZR2EH9ivoeUDnAgsz9PeCbx86kfSTlXHQf3fpX3sw%2ByPvkzdFGZLbetnVMrvT8rIWR3OpFTvo9Q1IoEpfCetG2Qu8iSPwj0Qqip4vDWpMaM8L46P02Fg%2FkGrH3EJ%2FucKTYWSGVeyP4K0qLnInFwbBkH%2Fhgf%2Bt1PcnB0XEoj7aan%2BF1Kr2DP%2FCQcz3hpM8KEnnegShP55ADMQIbUq1hQxlt%2B9p%2Fm%2F24MVl3mpLvWtnwDmaH%2F4o0Y%2FfZGUYgeAaLzan4%2FSLgwhHQNZpVwqlzOeaPrZQVcT8VyCXa7ZoHfQzXiKnT5ZDQ4mGqhsIMy7oDzivTZCA%2FpUGnE%2F3%2FJHp932MhoiA%2F0Z0YT%2Bx8EPEYfDsLxPagYwEeVsBljlgXiwbCHRsHnInpgdlJ6YBQk1UxnWxqBdH%2Bb60fo7wppdC1L%2Bqabj6oE2jTbER%2BSJhcEEN%2FRHurkTdOZVLnLJTXjL6Z6nV9dZJPhZVG8KWLV14mlQUj92p5RNW4R03ZhUcYnuq2YtHr7ZQ4IWZWjRpqDrAPwxV7hiWtsIbJB64HcY8a6PUZSa0004x51HB51MIrcNcw789e4Qb%2FttRkt8ilf2IIqDvikwsnFZlGnVhn692bQu8s3DNgUcnE6H8miBxYcSk%2BTYXetuul5vSP4%2BX%2BQfOCi360lrzky7qezhSspoDM2SHcJjzyx4tTa5191LXndtikbCMuS5vhJEYrTu4VRfAdUOewjgolmJs2G0e6BWTUBRrDA5oNvj6u6kO9NG%2F2paNevnMcod5VfGIBYKd%2F3VTAWtaCWaVNwQwaACwfGy23w4Jd%2BoqTOOLRuVUZwtP6GtCClVH3hX%2FyTnbJm9l1seV0ytVShnlqZXDZrMppCGivwExLD%2FBeT3TSmAOqbXFSTgN5WH5thystTJSwfbUArlV6NitWY3tYVOCriAfjF0EAgYtgK8%2FtwOi0Hhm%2FbusSkA37b2s3lgDQxOPBhJzWV0zL5lnxgXv1FsS%2B4BFulk6uIyXjT6XgDltsZqO9xCJswplCJISrviKSgAT6Ic8ayaQBcjA6ezKu9ufraHR%2BpmEQB08ISURWU3VbaiLFB6za7Pz3Ha3bizYTY1lKykAuHdJpmrfwldE%2FBtd9GkJeBoN6%2BURZdL5wM2suJ7Ig6vjmjzH270%2BGLFPvyyn8At1iUFfK1J2e8NY%2FSw5k0RaW%2FljGA5QdXTYAaB%2FFY8LWwyRTfDeA9GZKxpUsoSk7MOwugk5M0x4Hy5XMWQMnt9m290GMtdHsQlx8z0QUdhT0lKOi99FQ6MXolZPvxI%2FAEQd0fmJNTkOby69EM5PQtK2cqI8RQhpnKONilbk0daLU3OH7b%2BO%2FnUjMTo4EUc%2BPqzXRB63l5Hpftgrvw3j9whgmoOwLhhwl0hIM5yeasbfooAG2etNdtTd1uEhvypcnQGS6MGJZ4iR98No%2FNS6iOQiYgpLERCctGJ4u0IJwJoaIK7Daiga%2F%2Bu5V8S4rh62A%2BNLussr2w28QOGv9SC7%2FI5QDuXFWdDUbb%2FrorI2tTKWwEE0o79z%2FvKxhnrYCpa0Q5diTiUScRi0nMZekE1cicAizGBkL0PMghKTRbzLXwK1C%2FhA8zkbqRDJm0wyFMPcCGfY%2BgCA76PPVysBq46QHcFNG%2FhFYJ1pKr%2F8sLkdztVHdovMMzJ0b92yETItiwSUtTvse%2FGcU6Brxmk9o8F04xbG6SwOPc%2FqBvypjIaOEphE86K1zoMQM04bD0T0WEUvgu7WnxJhNBaoqGybmrQ01bnipo7X3x1cRSzBAIasB8F4aZ9mMs0%2FmyhCk7orDBr4h%2FA3NsQ7qGYo5A7ZPhsYJvt9LhNi%2BJ1RjEFM3WLcCQg7fpH0bwWN0BHRj4886MCtorSMGhevRuO98Vm091gj5Ip5ziv7dTvFdX1fPjEhWc%2BXqkL0%2FFKHLsvmiz4pHIItgtY%2B7mr%2B4G%2F0wPsKgwF97MK2%2BwN6jRlEKqvP1PzyRFMahSpTQGDxl6ZDtxE2WS8xQmuAYfilBNWkl7K1oK0xPUwkhMCbIGD08g2MaxC99bZTkIKRjeOrF4FOauQFvjz8iHhmUXIYMQgJQJmnXiVkx1RC%2B%2BQljGJul8NI9toZ36dqMDcwy79jAzmAEX5CdzHp1znc4N2eWcnJHDqQdhOVmAd0ZuzwjFbNnOOPpIFM4YOssne24m0bRc7nbgA0s4I11J33TUGSZQUuvUJOY9%2FYYg8j2SaIMAAIXP7m4cvl1NhDKe0hInv75T6apN%2B%2BTdbC%2BBRriDzzNNKdXWZOMwU0eO9l8VnCUGXtVGvwbHNP2vB7ryHHPbCjAdmLmNiwmKpcZ6Atn%2FnyGXE%2BYbVQtcRMtNagaoIA5tn5eVbazIuvSasvEO5p1fD6wXxuqixhBXoikRydfivlnBraicv6tVX7%2FmbmqbNbjJvI6rPdkndNuosdwKII3C4io%2FmaHRZ0YemQDPE2EVglGcgF8yk9toh%2FPdmx2YH4NWY6ukx%2FG3VdGcjlkHy3iag8ZL3v%2FU%2FoX5LfRe1vCKh%2B7kqh5SyB2rT3k5wIMXf7jihPVES93VnDpqWRGkV%2BffyO40akmOLlZcu%2F%2Fh6Sq5EWItmyemtOVXH9RKXk28PIkiBc0%2BiR9tcSNexrNqN1zZT11wozfUy6oIYP8Ek608qIeV%2FHNC9E8aSuhXWcFAPJ1Hpg0OltOKjMsShsUgn2yNnDs34LB9JHRBBg3L6beD5MGsSwfVUbv9gwiXOltAcPWs4XNoQvvlOb%2Flf%2BhMEFyRE2K7VSFx6iCurAb2XhxT7VGly3OQ%2FCMWmARsSG1pVOE1M7I81la79RPKGEGgsgfe2Bm%2Bb8TypVZ2QVH0KP0fDAPJeBsMKz6Mhivz8iPfQtF5F2W3t5ZBLqO5ZJDbU96ijVWotpNJBYRSvOe99i8P1ccwCVZO53UGs62NoPl25KvETAWgi1kYaOXUaPCtnnHEZyi7kmFwy5QiD2tBRn9gtm0Oquq7v1C2UPxto9D1F1DwQNhXLSxEoSby11yi%2FI5XoxMke%2FnVyzvyQNtrKiNuk4FUlBUi9lxK%2BRAXMDpHbnb1u%2BIq74Zog18kRlTOkhND58OColAg30ZlxSogTMitu7c9ptemPnN6cfRa%2FNwmmVtSYw1kfaknP2Gsm%2FJQVjDhJUk7ivaI8QQEFPesZfBQnOkGlhaNx7%2BhGbWHRqqvFE8LRmQ0ZHCF0jqrZLdKulT6gC%2BJ3%2BlVtH0jrgmwWyHSPIVOSTjZFmLN8GcBJm8rSy%2BHBjiAYRb5nxV6P5QhvqfWxgK%2B9vRWUQ3uuOj068JxbrsYpU23llf0PPWuWD%2BNFHGPLMnyGuTdng%2BeAs5qp3KW%2FA8xTt0KyTKPEP2yab734%2BKssH0%2FYptfBYz6iYKB%2BpLSRvtSrHntaUEP1AXfN4umsotscjlUtqPWj9%2BXkRfimOo6KCvW8ZlPOI9Ysi%2FpNyPyjpfwVMorwnpim1WCJqISejIOTxdSSfM5zVw5KqpHqFtzNp3cK2nTLvQu2N408FbZy7d1Fto8KYK9HxNVU%2FztgPQE%2Fl8IOvZgdM4uu3uvZ97iyoIav9K0Kn%2FrkcQh4zBvpizLjdDOcW1nsc1DBncY3Vpp229VTP17GHbibttpNU%2FgohkTZC3etvpA3nf8Cpc%2FWkEPXi%2FoociH3xP%2BVDpXLa8uuFv302skDqd495KrWWIPcW7e8%2FL9ENjKljmT3S11UmFSQYvUGjp4qMrHMUPiKdrPpK7l4WY5LzlF9q4OVEOgRsJ3OkpbAZPh9fnytRo1nTI52HqQKX%2BkNDfysk8IRZ9OVFhB8KwbhQFmhyJdAGmpTf%2BwV4FLtU6e9W21UiJ0xey%2FcTUifmT1JAVxEPXFB3agVJbnwSi9YdtbP0DQpTf36Ve1a1irMByaMp1KiSxHg7dcrr%2Fe9m2cqI0pyiq5aAoLgYhIKFGw4mctt%2B9koy%2F8o8c5U%2FLB0v6BCHe6vTjsEj4nF284VjPi6eOIBUk1m2qtar6FiVcdrpyUzpF%2B7MlYPfDfxXZPGOHincBvo2n5iuTn87t3TUIUwgMU3frvdR1t5JYgdhGhX%2BG6UeInZMVUcG87jfvJQErT6IF%2FmhTHI9NDyExFxQRVo%2B%2FBSH2zLXlN3sXFvPG04suqzDNBPt8m%2B0ZXhw2fP8TSfFQGD8bwXcG%2FTBgCCWYH80jpP8sBvb5DBhwoHmPVyQ4g%2FN8ifk3GUASgZCd8MBLYP0cZslX5D94maU1nkqM57EM64ngmfmSILdM3xaeKE%2FDtnepR5%2FA7V24j3svLirdoHWLTBM68p%2FtX%2BYOa7o3fs1%2FApEKBhUYlbjtSKyOwdFne8KKpUIDM2FTZGOuhUt%2FDsi0F69R6CRx%2FuoRhsP5fN2g6GHi89KVqWA4EHKRWqB0Kysc11j4DUt0BG5l7%2FxtMYGMSG9EnkjiJ8b2iK%2F3eKXB25mdxKh19cfgRwIcstn9JEJL759LQw%2Bg%2B1MTddcZquNME8DMvnJVchmCFnM9ehhg0Fc2M8m9ZZZYAKRRvszi44kbAafN28Ax2bDdtU5ny%2FkKgfNn78W19qQ%2Fbc4SuSWS5EcYTqxcGONZzS1lxFtRhkgfME2m7gPkRzHLtLGYiHjeFqEqL2i3tUoQarVVOXHqincKVltKjtmk9vVU7b5h0PcvCx0Kgv%2FrTqoEUPCO9nUKf3mOBji%2Bw0Lx9mtomqMuuXO7D6cVtjxXzQQLmmyRmUdjdA2SSD3t259sqwukTt9kjO3G40vIx9lRfYqpn%2BSZHjxnYOkx3fS8vQRcC%2FJg%2Be1LB7U14WVM7Ry8%2FpM5rgL3ivwQC7sb%2BfEC6j0atiGFIFFxI8jCfzcgR9Gi84mFEhu3SbFQ6eJKpapDB6Ws%2F2IhjrjCwTPt%2FD7PTZybpaTt32jiK1iKkKSls64tpbtWUXPDnxgnH5%2B9meu2PjIoMt29aK6Ec6vkjhssUWe%2BPJ3mad7PtvEU4aM188e29qYSyjh8XvphewAsQ8AJzE0vG8CCD0GTX30rbq8NuRnUYNS7IIG85PdxyF3l1ad0u%2F40WfkcqoQC%2FEDx1AWNd8frlZ5P30g1K6rpv%2B2Fq9h94SEfod69uKE5U0ArTJi0kIcsfMc%2B6JTp1VSDKsaHswL7Ay%2BewwDMaf%2BIXfWrjF8OfELrwva7JYPkcF7HQw%2FukbrnKWfDZz2I%2BpEcqWjnS8cSuDo7U2ZuPDJSFF1O%2Fx%2B0yiK9AGS1cPlym%2BMNC47NIaifpgroL5CIbjS1mVL8fOzkdoJzXXTtqwgw7baxHrPHmI3HP1iisz1xDKRICPECa3XUQJb%2FlReywhVIBaV25bDsX2wNlqgw6Q%2Fg8O78NO3UKRj9ypK2W3K6EfdGPLY1L95rAPyvhV9VXeWjGu4iVXnWixSZDJqJ1NS0NHfiyHUWUN0L6yKl1kBdkLsyEhBOBIufFvo0p9ax8Cjj%2Buwyy%2BfQApxX%2B%2Bn4SejIGh%2B%E2%80%A6

- TODO => link to Config examples




## README "QNH.Overheid.KernRegister.Beheer"
APP_DATA and Logs folders need read and write permissions for Elmah logging.
